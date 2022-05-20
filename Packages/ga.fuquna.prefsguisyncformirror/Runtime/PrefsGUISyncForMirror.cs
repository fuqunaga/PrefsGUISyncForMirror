using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace PrefsGUI.Sync
{
    /// <summary>
    /// Sync PrefsGUI parameter over UNET
    /// </summary>
    [DefaultExecutionOrder(-1)]
    public class PrefsGUISyncForMirror : NetworkBehaviour
    {
        private readonly SyncDictionary<string, byte[]> _syncDictionary = new();
        private readonly Dictionary<string, object> _cachedObjDictionary = new();

        // want use HashSet but use List so it will be serialized on Inspector
        public List<string> ignoreKeys = new();

        [SyncVar] bool materialPropertyDebugMenuUpdate;

        #region Unity
        
        public void Awake()
        {
            syncInterval = 0f;
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            // ignore when "Host"
            if (!NetworkServer.active)
            {
                _syncDictionary.Callback += (_, key, _) => _cachedObjDictionary.Remove(key);
                ReadPrefs(true);
            }
        }

        public void Update()
        {
            SendPrefs();
            
            // ignore when "Host"
            if (!NetworkServer.active)
            {
                ReadPrefs();
            }
        }
        
        #endregion
        

        [ServerCallback]
        void SendPrefs()
        {
            foreach (var prefs in PrefsParam.all)
            {
                var key = prefs.key;
                if (ignoreKeys.Contains(key)) continue;

                var obj = prefs.GetObject();
                if (obj == null) continue;

                WriteObj(key, obj);
            }

            if (materialPropertyDebugMenuUpdate != MaterialPropertyDebugMenu.update)
            {
                materialPropertyDebugMenuUpdate = MaterialPropertyDebugMenu.update;
            }
        }



        void WriteObj(string key, object obj)
        {
            if (_cachedObjDictionary.TryGetValue(key, out var cachedObj) && cachedObj.Equals(obj))
            {
                return;
            }

            _cachedObjDictionary[key] = obj;

            var type = obj.GetType();

            _syncDictionary.TryGetValue(key, out var bytes);
            BytesConverter.ObjToBytes(type, obj, ref bytes);

            _syncDictionary[key] = bytes;
        }
        

        [ClientCallback]
        void ReadPrefs(bool checkAlreadyGet = false)
        {
            var alreadyGet = false;
            var alreadyGetFunc = checkAlreadyGet
                ? () => alreadyGet = true
                : (Action) null;

            var allDic = PrefsParam.allDic;
            foreach (var (key, bytes)  in _syncDictionary)
            {
                if (!allDic.TryGetValue(key, out var prefs)) continue;
                
                if (!_cachedObjDictionary.TryGetValue(key, out var obj))
                {
                    obj = BytesConverter.BytesToObj(prefs.GetInnerType(), bytes);
                    _cachedObjDictionary[key] = obj;
                }
                    
                alreadyGet = false;
                prefs.SetSyncedObject(obj, alreadyGetFunc);

                if (alreadyGet)
                {
                    Debug.LogWarning(
                        $"key:[{prefs.key}] Get() before synced. before:[{prefs.GetObject()}] sync:[{obj}]");
                }
            }
            

            MaterialPropertyDebugMenu.update = materialPropertyDebugMenuUpdate;
        }


    }
}