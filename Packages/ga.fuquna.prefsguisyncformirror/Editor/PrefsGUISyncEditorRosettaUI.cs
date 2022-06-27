#if PrefsGUI_RosettaUI

using System.Collections.Generic;
using System.Linq;
using PrefsGUI.RosettaUI.Editor;
using RosettaUI;
using UnityEditor;

namespace PrefsGUI.Sync.Editor
{
    [InitializeOnLoad]
    public class PrefsGUISyncEditorRosettaUI
    {
        static PrefsGUISyncEditorRosettaUI()
        {
            PrefsGUIEditorRosettaUI.RegisterObjCheckExtension(new PrefsGUIEditorRosettaUIObjCheckExtension());
        }

        private class PrefsGUIEditorRosettaUIObjCheckExtension : IPrefsGUIEditorRosettaUIObjCheckExtension
        {
            public Element Title() => UI.Label("Sync");

            public Element PrefsLeft(PrefsParam prefs)
            {
                return UI.Field(null, 
                    () => PrefsGUISyncEditorUtility.GetSyncFlag(prefs.key),
                    flag => PrefsGUISyncEditorUtility.SetSyncFlag(prefs.key, flag)
                    );
            }

            public Element PrefsSetLeft(IEnumerable<PrefsParam> prefsList)
            {
                return UI.Field(null, 
                    () => prefsList.Any(prefs => PrefsGUISyncEditorUtility.GetSyncFlag(prefs.key)),
                    flag => PrefsGUISyncEditorUtility.SetSyncFlags(prefsList.Select(prefs => prefs.key), flag)
                );
            }
        }
    }
}

#endif