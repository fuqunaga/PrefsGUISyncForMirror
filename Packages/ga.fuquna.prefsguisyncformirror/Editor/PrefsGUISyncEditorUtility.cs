using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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
                    if (_sync == null)
                    {
                        Debug.LogWarning($"nameof(PrefsGUISyncForMirror) is not found");
                    }
                }

                return _sync;
            }
        }

        public static IEnumerable<string> IgnoreKeys => Sync.IgnoreKeys;

        public static bool GetSyncFlag(string key) => !Sync.HasIgnoreKey(key); 
        
        public static void SetSyncFlag(string key, bool syncFlag)
        {
            RegisterUndoAndSetDirty(Sync);

            if (!syncFlag) 
                Sync.AddIgnoreKey(key);
            else
                Sync.RemoveIgnoreKey(key);
        }
        
        public static void SetSyncFlags(IEnumerable<string> keys, bool syncFlag)
        {
            RegisterUndoAndSetDirty(Sync);
            
            if (!syncFlag)
                foreach (var key in keys)
                    Sync.AddIgnoreKey(key);
            else
                foreach (var key in keys)
                    Sync.RemoveIgnoreKey(key);
        }

        static void RegisterUndoAndSetDirty(Object sync)
        {
            Undo.RecordObject(sync, "PrefsGUISync change sync flag");
            EditorUtility.SetDirty(sync);
        }
    }
}