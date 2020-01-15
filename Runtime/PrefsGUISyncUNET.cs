using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;

#pragma warning disable 0618  

namespace PrefsGUI.Sync.UNET
{
    /// <summary>
    /// Sync PrefsGUI parameter over UNET
    /// </summary>
    [DefaultExecutionOrder(-1)]
    public partial class PrefsGUISyncUNET : NetworkBehaviour
    {
        #region Sync

        SyncListKeyBool syncListKeyBool = new SyncListKeyBool();
        SyncListKeyInt syncListKeyInt = new SyncListKeyInt();
        SyncListKeyUInt syncListKeyUInt = new SyncListKeyUInt();
        SyncListKeyFloat syncListKeyFloat = new SyncListKeyFloat();
        SyncListKeyString syncListKeyString = new SyncListKeyString();
        SyncListKeyColor syncListKeyColor = new SyncListKeyColor();
        SyncListKeyVector2 syncListKeyVector2 = new SyncListKeyVector2();
        SyncListKeyVector3 syncListKeyVector3 = new SyncListKeyVector3();
        SyncListKeyVector4 syncListKeyVector4 = new SyncListKeyVector4();
        SyncListKeyVector2Int syncListKeyVector2Int = new SyncListKeyVector2Int();
        SyncListKeyVector3Int syncListKeyVector3Int = new SyncListKeyVector3Int();
        SyncListKeyRect syncListKeyRect = new SyncListKeyRect();
        SyncListKeyBounds syncListKeyBounds = new SyncListKeyBounds();
        SyncListKeyBoundsInt syncListKeyBoundsInt = new SyncListKeyBoundsInt();

        [SyncVar] bool materialPropertyDebugMenuUpdate;

        #endregion


        Dictionary<Type, ISyncListKeyObj> typeToSyncList;
        Dictionary<string, int> keyToIndex = new Dictionary<string, int>();

        public readonly List<string> ignoreKeys = new List<string>(); // want use HashSet but use List so it will be serialized on Inspector


        public void Awake()
        {
            typeToSyncList = new ISyncListKeyObj[]{
                SyncListKeyObjHelper.Create(syncListKeyBool),
                SyncListKeyObjHelper.Create(syncListKeyInt),
                SyncListKeyObjHelper.Create(syncListKeyUInt),
                SyncListKeyObjHelper.Create(syncListKeyFloat),
                SyncListKeyObjHelper.Create(syncListKeyString),
                SyncListKeyObjHelper.Create(syncListKeyColor),
                SyncListKeyObjHelper.Create(syncListKeyVector2),
                SyncListKeyObjHelper.Create(syncListKeyVector3),
                SyncListKeyObjHelper.Create(syncListKeyVector4),
                SyncListKeyObjHelper.Create(syncListKeyRect),
                SyncListKeyObjHelper.Create(syncListKeyVector2Int),
                SyncListKeyObjHelper.Create(syncListKeyVector3Int),
                SyncListKeyObjHelper.Create(syncListKeyBounds),
                SyncListKeyObjHelper.Create(syncListKeyBoundsInt)
            }
            .ToDictionary(sl => sl.ValueType);
        }


        public override void OnStartClient()
        {
            base.OnStartClient();
            ReadPrefs(true);
        }


        public void Start()
        {
            SendPrefs();
        }

        public void Update()
        {
            SendPrefs();
            ReadPrefs();
        }

        static PrefsParam[] tmpPrefsArray = new PrefsParam[0];

        [ServerCallback]
        void SendPrefs()
        {
            foreach (var prefs in PrefsParam.all)
            {
                var key = prefs.key;

                if (!ignoreKeys.Contains(key))
                {
                    var obj = prefs.GetObject();
                    if (obj != null)
                    {
                        var type = prefs.GetInnerType();
                        if (type.IsEnum)
                        {
                            type = typeof(int);
                            obj = Convert.ToInt32(obj);
                        }

                        if (keyToIndex.TryGetValue(key, out var index))
                        {
                            var sl = typeToSyncList[type];
                            sl.Set(index, key, obj);
                        }
                        else
                        {
                            if (typeToSyncList.ContainsKey(type))
                            {
                                Assert.IsTrue(typeToSyncList.ContainsKey(type),
                                    string.Format($"type [{type}] is not supported."));

                                var iSynList = typeToSyncList[type];
                                var idx = iSynList.Count;
                                iSynList.Add(key, obj);
                                //keyToIndex[key] = new KeyData() { type = type, idx = idx, objCache = obj };
                                keyToIndex[key] = idx;
                            }
                        }
                    }
                }
            }

            if (materialPropertyDebugMenuUpdate != MaterialPropertyDebugMenu.update)
            {
                materialPropertyDebugMenuUpdate = MaterialPropertyDebugMenu.update;
            }
        }


        List<ISyncListKeyObj> syncLists;

        [ClientCallback]
        void ReadPrefs(bool checkAlreadyGet = false)
        {
            // ignore at "Host"
            if (!NetworkServer.active)
            {
                if (syncLists == null)
                {
                    syncLists = typeToSyncList.Values.ToList();
                }

                var alreadyGet = false;
                var alreadyGetFunc = checkAlreadyGet 
                    ? () => alreadyGet = true
                    : (Action)null;

                var allDic = PrefsParam.allDic;
                foreach (var sl in syncLists)
                {
                    for (var i = 0; i < sl.Count; ++i)
                    {
                        var (key, obj) = sl.Get(i);

                        if (allDic.TryGetValue(key, out var prefs))
                        {
                            alreadyGet = false;
                            prefs.SetSyncedObject(obj, alreadyGetFunc);

                            if ( alreadyGet )
                            {
                                Debug.LogWarning($"key:[{prefs.key}] Get() before synced. before:[{prefs.GetObject()}] sync:[{obj}]");
                            }
                        }
                    }
                }
            }

            MaterialPropertyDebugMenu.update = materialPropertyDebugMenuUpdate;
        }
    }
}