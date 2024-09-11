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
            PrefsGUIEditorWindowRosettaUI.RegisterObjCheckExtension(new PrefsGUIEditorRosettaUIObjCheckExtension());
        }

        private class PrefsGUIEditorRosettaUIObjCheckExtension : IPrefsGUIEditorRosettaUIObjCheckExtension
        {
            public Element Title() => UI.Label("Sync");

            public Element PrefsLeft(PrefsParam prefs)
            {
                return BringToTop(
                    UI.Toggle(null,
                        () => PrefsGUISyncEditorUtility.GetSyncFlag(prefs.key),
                        flag => PrefsGUISyncEditorUtility.SetSyncFlag(prefs.key, flag)
                    )
                );
            }

            public Element PrefsSetLeft(IEnumerable<PrefsParam> prefsList)
            {
                return BringToTop(
                    UI.Toggle(null,
                        () => prefsList.Any(prefs => PrefsGUISyncEditorUtility.GetSyncFlag(prefs.key)),
                        flag => PrefsGUISyncEditorUtility.SetSyncFlags(prefsList.Select(prefs => prefs.key), flag)
                    )
                );
            }

            private static Element BringToTop(Element element)
            {
                return UI.Column(
                    element,
                    UI.Space()
                ).SetFlexGrow(0); // UI.Row()だと横に広がってしまうのを抑制
            }
        }
    }
}

#endif