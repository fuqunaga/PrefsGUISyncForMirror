using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PrefsGUI.Sync.Editor
{
    public static class PrefsGUISyncEditorUtility
    {
        private static PrefsGUISyncForMirror _sync;
        private static PrefsGUISyncForMirror Sync
        {
            get
            {
                if (_sync == null)
                {
                    _sync = Object.FindObjectOfType<PrefsGUISyncForMirror>();
                }

                return _sync;
            }
        }

        public static IEnumerable<string> IgnoreKeys => Sync != null ? Sync.IgnoreKeys : Array.Empty<string>();

        public static bool GetSyncFlag(string key) => Sync != null && !Sync.HasIgnoreKey(key); 
        
        public static void SetSyncFlag(string key, bool syncFlag)
        {
            if (Sync == null)
            {
                Debug.LogWarning($"{nameof(PrefsGUISyncForMirror)} is not found.");
                return;
            }
            
            RegisterUndoAndSetDirty(Sync);

            if (!syncFlag) 
                Sync.AddIgnoreKey(key);
            else
                Sync.RemoveIgnoreKey(key);
        }
        
        public static void SetSyncFlags(IEnumerable<string> keys, bool syncFlag)
        {
            if (Sync == null)
            {
                Debug.LogWarning($"{nameof(PrefsGUISyncForMirror)} is not found.");
                return;
            }
            
            RegisterUndoAndSetDirty(Sync);
            
            if (!syncFlag)
                foreach (var key in keys)
                    Sync.AddIgnoreKey(key);
            else
                foreach (var key in keys)
                    Sync.RemoveIgnoreKey(key);
        }

        private static void RegisterUndoAndSetDirty(Object sync)
        {
            Undo.RecordObject(sync, "PrefsGUISync change sync flag");
            EditorUtility.SetDirty(sync);
        }
    }
}