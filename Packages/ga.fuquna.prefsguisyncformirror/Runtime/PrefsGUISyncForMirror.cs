using System;
using System.Collections.Generic;
using System.Reflection;
using Mirror;
using UnityEngine;
using UnityEngine.Pool;

namespace PrefsGUI.Sync
{
    /// <summary>
    /// Sync PrefsGUI parameter over UNET
    /// </summary>
    [DefaultExecutionOrder(-1)]
    public class PrefsGUISyncForMirror : NetworkBehaviour
    {
        // want use HashSet but use List so it will be serialized on Inspector
        public List<string> ignoreKeys = new();

        private readonly SyncDictionary<string, byte[]> _syncDictionary = new();
        private HashSet<string> _receivedKey;

        
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
                _receivedKey = new(_syncDictionary.Keys);
                _syncDictionary.Callback += (op, key, _) =>
                {
                    switch (op)
                    {
                        case SyncIDictionary<string, byte[]>.Operation.OP_ADD:
                        case SyncIDictionary<string, byte[]>.Operation.OP_SET:
                            _receivedKey.Add(key);
                            break;

                        case SyncIDictionary<string, byte[]>.Operation.OP_CLEAR:
                        case SyncIDictionary<string, byte[]>.Operation.OP_REMOVE:
                            _receivedKey.Remove(key);
                            break;
                    }
                };
                
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

                WritePrefsToSyncDictionary(prefs);
            }
        }

        private readonly Dictionary<Type, Action<PrefsParam>> _toDictionaryTable = new();

        private static readonly MethodInfo CreateToDictionaryMethodInfo =
            typeof(PrefsGUISyncForMirror).GetMethod(nameof(CreateToDictionaryAction),
                BindingFlags.Instance | BindingFlags.NonPublic);

        void WritePrefsToSyncDictionary(PrefsParam prefs)
        {
            var innerType = prefs.GetInnerType();
            if (!_toDictionaryTable.TryGetValue(innerType, out var action))
            {
                action = (Action<PrefsParam>)CreateToDictionaryMethodInfo.MakeGenericMethod(innerType).Invoke(this, null);
                _toDictionaryTable[innerType] = action;
            }

            action(prefs);
        }

        Action<PrefsParam> CreateToDictionaryAction<T>()
        {
            var toByteDictionary = new PrefsToByteDictionary<T>(_syncDictionary);
            return (prefs) => toByteDictionary.WriteFrom(prefs.GetInnerAccessor<T>());
        }
        
        

        [ClientCallback]
        void ReadPrefs(bool checkAlreadyGet = false)
        {
            if ( _receivedKey == null) return;
            
            var allDic = PrefsParam.allDic;

            using var _ = ListPool<string>.Get(out var removeKeys);

            foreach (var key in _receivedKey)
            {
                if (!allDic.TryGetValue(key, out var prefs)) continue;
                removeKeys.Add(key);

                var bytes = _syncDictionary[key];
                var valueChangedAndAlreadyGet = WriteBytesToPrefs(bytes, prefs);
                
                if (checkAlreadyGet && valueChangedAndAlreadyGet)
                {
                    Debug.LogWarning($"key:[{prefs.key}] Prefs.Get() called before sync. Application may be using pre-sync values.");
                }
            }
            
            _receivedKey.ExceptWith(removeKeys);
        }

        
        private readonly Dictionary<Type, SetToPrefsFunc> _setToPrefsFuncTable = new();

        private readonly MethodInfo _createToPrefsActionMethodInfo =
            typeof(PrefsGUISyncForMirror).GetMethod(nameof(CreateSetToPrefsFunc),
                BindingFlags.NonPublic | BindingFlags.Static);

        bool WriteBytesToPrefs(byte[] bytes, PrefsParam prefs)
        {
            var innerType = prefs.GetInnerType();
            if (!_setToPrefsFuncTable.TryGetValue(innerType, out var func))
            {
                func = (SetToPrefsFunc)_createToPrefsActionMethodInfo.MakeGenericMethod(innerType).Invoke(null, null);
                _setToPrefsFuncTable[innerType] = func;
            }

            return func(bytes, prefs);
        }

        // return Already Get()'d and value update 
        delegate bool SetToPrefsFunc(byte[] bytes, PrefsParam prefs);
        static SetToPrefsFunc CreateSetToPrefsFunc<T>()
        {
            return (bytes, prefs) =>
            {
                var value = BytesConverter.BytesToValue<T>(bytes);
                var innerAccessor = prefs.GetInnerAccessor<T>();
                
                var alreadyGet = innerAccessor.IsAlreadyGet;
                return alreadyGet && innerAccessor.SetSyncedValue(value);
            };
        }
    }
}