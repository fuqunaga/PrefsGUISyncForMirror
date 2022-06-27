#if PrefsGUI_RapidGUI

using System.Collections.Generic;
using System.Linq;
using PrefsGUI.RapidGUI.Editor;
using RapidGUI;
using UnityEditor;
using UnityEngine;

namespace PrefsGUI.Sync.Editor
{
    [InitializeOnLoad]
    public class PrefsGUISyncEditor : IPrefsGUIEditorRapidGUIExtension
    {
        #region static

        static PrefsGUISyncEditor()
        {
            var instance = new PrefsGUISyncEditor();
            PrefsGUIEditorRapidGUI.RegisterExtension(instance);
        }

        #endregion


        private PrefsGUISyncForMirror _sync;


        public void GUIHeadLine()
        {
            _sync = Object.FindObjectOfType<PrefsGUISyncForMirror>();
            if (_sync != null) GUILayout.Label("Sync");
        }

        public void GUIPrefsLeft(PrefsParam prefs)
        {
            SyncToggle(_sync, prefs);
        }

        public void GUIGroupLabelLeft(IEnumerable<PrefsParam> prefsList)
        {
            SyncToggleList(_sync, prefsList);
        }


        static void SyncToggle(PrefsGUISyncForMirror sync, PrefsParam prefs)
        {
            if (sync != null)
            {
                using (new RGUI.EnabledScope(!Application.isPlaying))
                {
                    var key = prefs.key;
                    var isSync = !sync.ignoreKeys.Contains(key);

                    if (isSync != GUILayout.Toggle(isSync, GUIContent.none, PrefsGUIEditorRapidGUIBase.ToggleWidth))
                    {
                        Undo.RecordObject(sync, "Change PrefsGUI sync flag");
                        EditorUtility.SetDirty(sync);

                        if (isSync) sync.ignoreKeys.Add(key);
                        else sync.ignoreKeys.Remove(key);
                    }
                }
            }
        }

        static void SyncToggleList(PrefsGUISyncForMirror sync, IEnumerable<PrefsParam> prefsList)
        {
            if (sync != null)
            {
                var keys = prefsList.Select(p => p.key).ToList();
                var syncKeys = keys.Except(sync.ignoreKeys);

                var isSync = PrefsGUIEditorRapidGUIBase.ToggleMixed(syncKeys.Count(), keys.Count);
                if (isSync.HasValue)
                {
                    Undo.RecordObject(sync, "Change PrefsGUIs sync flag");
                    EditorUtility.SetDirty(sync);

                    if (!isSync.Value)
                    {
                        sync.ignoreKeys.AddRange(syncKeys);
                    }
                    else
                    {
                        foreach (var key in keys)
                        {
                            sync.ignoreKeys.Remove(key);
                        }
                    }
                }
            }
        }
    }
}

#endif