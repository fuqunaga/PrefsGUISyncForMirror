﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mirror;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;

namespace PrefsGUI.Sync
{
    /// <summary>
    /// Sync PrefsGUI parameter over UNET
    /// </summary>
    [DefaultExecutionOrder(-1)]
    public class PrefsGUISyncForMirror : NetworkBehaviourDivideSpawnData, ISerializationCallbackReceiver
    {
        [Tooltip("Prefs that have never been changed from their default values will not be synced.")]
        public bool syncOnlyValueChangedPrefs = true;
        
        // want use HashSet but use List so it will be serialized on Inspector
        [SerializeField]
        [FormerlySerializedAs("ignoreKeys")]
        private List<string> ignoreKeyList = new();
        
        private HashSet<string> _ignoreKeys;
        private Dictionary<PrefsParam, Action> _prefsAndCallbackTable;
        private readonly HashSet<PrefsParam> _sendPrefsSet = new();
        private readonly List<PrefsParam> _newPrefsList = new();
        private HashSet<string> _receivedKeys;
        
        private readonly SyncDictionary<string, byte[]> _syncDictionary = new();
        private List<SyncObject> _divideSpawnDataSyncObjects;
        
        public IEnumerable<string> IgnoreKeys => ignoreKeyList;

        protected override IEnumerable<SyncObject> DivideTargetSyncObjects =>
            _divideSpawnDataSyncObjects ??= new List<SyncObject> { _syncDictionary };
        
        
        #region Unity
        
        public void Awake()
        {
            syncInterval = 0f;
        }


        public void Update()
        {
            SendPrefs();
            
            if (isClientOnly)
            {
                ReadPrefs();
            }
        }

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            _ignoreKeys = new(ignoreKeyList);
        }
        
        #endregion
        
        
        #region Mirror
        
        public override void OnStartServer()
        {
            base.OnStartServer();
            RegisterPrefsCallbacks();
        }

        public override void OnStopServer()
        {
            base.OnStopServer();
            ClearPrefsCallbacks();
        }

        public override void OnStartClient()
        {
            // base.OnStartClient()内でonSpawnFinishedが呼ばれることがあるためメソッドの順番注意
            if (isClientOnly)
            {
                onSpawnFinished.AddListener(OnSpawnFinished);
            }

            base.OnStartClient();
        }
        
        #endregion
        
        
        #region IgnoreKey

        public bool HasIgnoreKey(string key) => _ignoreKeys.Contains(key);

        public void AddIgnoreKey(string key)
        {
            ignoreKeyList.Add(key);
            _ignoreKeys.Add(key);
        }

        public void RemoveIgnoreKey(string key)
        {
            ignoreKeyList.RemoveAll(k => k == key);
            _ignoreKeys.Remove(key);
        }

        #endregion
        

        #region Server

        private void RegisterPrefsCallbacks()
        {
            var existingPrefsEnumerable = PrefsParam.all.Where(prefs => !_ignoreKeys.Contains(prefs.key));
            
            foreach (var prefs in existingPrefsEnumerable)
            {
                RegisterPrefsValueChangedCallback(prefs);
            }
            
            PrefsParam.onRegisterPrefsParam += OnRegisterPrefsParam;
        }

        private void RegisterPrefsValueChangedCallback(PrefsParam prefs)
        {
            _prefsAndCallbackTable ??= new Dictionary<PrefsParam, Action>();
            var success = _prefsAndCallbackTable.TryAdd(prefs, ValueChangedCallback);
            if (!success) return;

            prefs.RegisterValueChangedCallback(ValueChangedCallback);
            
            // ここで prefs.IsDefault はエラーになる
            // PrefsParam.onRegisterPrefsParam 経由で呼ばれるとAwake()内等などで、prefsのデフォルト値へのアクセスがエラーになるっぽい
            //
            // var needSend = !prefs.IsDefault || !syncOnlyValueChangedPrefs;
            // if (needSend)
            // {
            //     ValueChangedCallback();
            // }
            _newPrefsList.Add(prefs);

            return;

            void ValueChangedCallback()
            {
                _sendPrefsSet.Add(prefs);
            }
        }
        
        private void OnRegisterPrefsParam(PrefsParam prefs)
        {
            if (_ignoreKeys.Contains(prefs.key)) return;
            RegisterPrefsValueChangedCallback(prefs);
        }
        
        private void ClearPrefsCallbacks()
        {
            if (_prefsAndCallbackTable != null)
            {
                foreach (var (prefs, callback) in _prefsAndCallbackTable)
                {
                    prefs.UnregisterValueChangedCallback(callback);
                }
                
                _prefsAndCallbackTable.Clear();
            }
            
            PrefsParam.onRegisterPrefsParam -= OnRegisterPrefsParam;
        }
        

        [ServerCallback]
        private void SendPrefs()
        {
            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var prefs in _newPrefsList)
            {
                var needSend = !syncOnlyValueChangedPrefs || !prefs.IsDefault;
                if (!needSend) continue;
                
                _sendPrefsSet.Add(prefs);
            }
            _newPrefsList.Clear();

            
            foreach (var prefs in _sendPrefsSet)
            {
                WritePrefsToSyncDictionary(prefs);
            }
            _sendPrefsSet.Clear();
        }


        private readonly Dictionary<Type, Action<PrefsParam>> _toDictionaryTable = new();

        private static readonly MethodInfo CreateToDictionaryMethodInfo =
            typeof(PrefsGUISyncForMirror).GetMethod(nameof(CreateToDictionaryAction),
                BindingFlags.Instance | BindingFlags.NonPublic);

        private void WritePrefsToSyncDictionary(PrefsParam prefs)
        {
            var innerType = prefs.GetInnerType();
            if (!_toDictionaryTable.TryGetValue(innerType, out var action))
            {
                action = (Action<PrefsParam>)CreateToDictionaryMethodInfo.MakeGenericMethod(innerType).Invoke(this, null);
                _toDictionaryTable[innerType] = action;
            }

            action(prefs);
        }

        private Action<PrefsParam> CreateToDictionaryAction<T>()
        {
            var toByteDictionary = new PrefsToByteDictionary<T>(_syncDictionary);
            return (prefs) => toByteDictionary.WriteFrom(prefs.GetInnerAccessor<T>());
        }
        
        #endregion


        #region Client

        private void OnSpawnFinished()
        {
            _receivedKeys = new HashSet<string>(_syncDictionary.Keys);

            // If you get an error with Mirror 89.*, please update to 89.4 or later.
            // No way to know minor version in code.
            // - https://github.com/MirrorNetworking/Mirror/releases/tag/v89.4.0
#if MIRROR_89_OR_NEWER
            _syncDictionary.OnAdd += (key) => _receivedKeys.Add(key);
            _syncDictionary.OnSet += (key, _) => _receivedKeys.Add(key);
            _syncDictionary.OnRemove += (key, _) => _receivedKeys.Remove(key);
            _syncDictionary.OnClear += () => _receivedKeys.Clear();
#else
            _syncDictionary.Callback += (op, key, _) =>
            {
                switch (op)
                {
                    case SyncIDictionary<string, byte[]>.Operation.OP_ADD:
                    case SyncIDictionary<string, byte[]>.Operation.OP_SET:
                        _receivedKeys.Add(key);
                        break;

                    case SyncIDictionary<string, byte[]>.Operation.OP_CLEAR:
                    case SyncIDictionary<string, byte[]>.Operation.OP_REMOVE:
                        _receivedKeys.Remove(key);
                        break;
                }
            };
#endif
                
            ReadPrefs(true);
        }

        [ClientCallback]
        private void ReadPrefs(bool checkAlreadyGet = false)
        {
            if ( _receivedKeys == null) return;
            
            var allDic = PrefsParam.allDic;

            using var _ = ListPool<string>.Get(out var removeKeys);

            foreach (var key in _receivedKeys)
            {
                if (!allDic.TryGetValue(key, out var prefs)) continue;
                removeKeys.Add(key);

                var bytes = _syncDictionary[key];
                var valueChangedAndAlreadyGet = WriteBytesToPrefs(bytes, prefs);
                
                if (checkAlreadyGet && valueChangedAndAlreadyGet)
                {
                    Debug.LogWarning($"key:[{prefs.key}] Get() called before it was rewritten in sync. The application may be using pre-sync values, consider using PrefsGUISync.IsSpawnFinished or onSpawnFinished event.");
                }
            }
            
            _receivedKeys.ExceptWith(removeKeys);
        }

        
        private readonly Dictionary<Type, SetToPrefsFunc> _setToPrefsFuncTable = new();

        private readonly MethodInfo _createToPrefsActionMethodInfo =
            typeof(PrefsGUISyncForMirror).GetMethod(nameof(CreateSetToPrefsFunc),
                BindingFlags.NonPublic | BindingFlags.Static);

        private bool WriteBytesToPrefs(byte[] bytes, PrefsParam prefs)
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
        private delegate bool SetToPrefsFunc(byte[] bytes, PrefsParam prefs);

        private static SetToPrefsFunc CreateSetToPrefsFunc<T>()
        {
            return (bytes, prefs) =>
            {
                var value = BytesConverter.BytesToValue<T>(bytes);
                var innerAccessor = prefs.GetInnerAccessor<T>();
                
                var alreadyGet = innerAccessor.IsAlreadyGet;
                var valueUpdated = innerAccessor.SetSyncedValue(value);

                return alreadyGet && valueUpdated;
            };
        }

        #endregion
    }
}