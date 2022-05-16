using PrefsGUI.Editor;
using RapidGUI;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace PrefsGUI.Sync.UNET.Editor
{
    [InitializeOnLoad]
    public class PrefsGUISyncEditor : IPrefsGUIEditorExtension
    {
        #region static

        static PrefsGUISyncEditor()
        {
            var instance = new PrefsGUISyncEditor();
            PrefsGUIEditor.RegistExtension(instance);
        }

        #endregion


        private PrefsGUISyncUNET sync;


        public void GUIHeadLine()
        {
            sync = Object.FindObjectOfType<PrefsGUISyncUNET>();
            if (sync != null) GUILayout.Label("Sync");
        }

        public void GUIPrefsLeft(PrefsParam prefs)
        {
            SyncToggle(sync, prefs);
        }

        public void GUIGroupLabelLeft(IEnumerable<PrefsParam> prefsList)
        {
            SyncToggleList(sync, prefsList);
        }


        void SyncToggle(PrefsGUISyncUNET sync, PrefsParam prefs)
        {
            if (sync != null)
            {
                using (new RGUI.EnabledScope(!Application.isPlaying))
                {
                    var key = prefs.key;
                    var isSync = !sync.ignoreKeys.Contains(key);

                    if (isSync != GUILayout.Toggle(isSync, GUIContent.none, PrefsGUIEditorBase.ToggleWidth))
                    {
                        Undo.RecordObject(sync, "Change PrefsGUI sync flag");
                        EditorUtility.SetDirty(sync);

                        if (isSync) sync.ignoreKeys.Add(key);
                        else sync.ignoreKeys.Remove(key);
                    }
                }
            }
        }

        void SyncToggleList(PrefsGUISyncUNET sync, IEnumerable<PrefsParam> prefsList)
        {
            if (sync != null)
            {
                var keys = prefsList.Select(p => p.key).ToList();
                var syncKeys = keys.Except(sync.ignoreKeys);

                var isSync = PrefsGUIEditorBase.ToggleMixed(syncKeys.Count(), keys.Count);
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