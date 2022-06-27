#if PrefsGUI_RapidGUI

using System.Collections.Generic;
using System.Linq;
using PrefsGUI.RapidGUI.Editor;
using RapidGUI;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;

namespace PrefsGUI.Sync.Editor
{
    [InitializeOnLoad]
    public class PrefsGUISyncEditorRapidGUI : IPrefsGUIEditorRapidGUIExtension
    {
        #region static

        static PrefsGUISyncEditorRapidGUI()
        {
            var instance = new PrefsGUISyncEditorRapidGUI();
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
            using (new RGUI.EnabledScope(!Application.isPlaying))
            {
                var key = prefs.key;
                var isSync = PrefsGUISyncEditorUtility.GetSyncFlag(key);

                if (isSync != GUILayout.Toggle(isSync, GUIContent.none, PrefsGUIEditorRapidGUIBase.ToggleWidth))
                {
                    PrefsGUISyncEditorUtility.SetSyncFlag(key, !isSync);
                }
            }
        }

        public void GUIGroupLabelLeft(IEnumerable<PrefsParam> prefsList)
        { 
            using var listPool = ListPool<string>.Get(out var keys);
            keys.AddRange(prefsList.Select(p => p.key));

            var syncKeys = keys.Except(PrefsGUISyncEditorUtility.IgnoreKeys);

            var isSync = PrefsGUIEditorRapidGUIBase.ToggleMixed(syncKeys.Count(), keys.Count);
            if (isSync.HasValue)
            {
                PrefsGUISyncEditorUtility.SetSyncFlags(keys, isSync.Value);
            }
        }
    }
}

#endif