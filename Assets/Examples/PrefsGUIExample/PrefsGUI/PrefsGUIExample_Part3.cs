using UnityEngine;
#if PrefsGUI_RapidGUI
using RapidGUI;
using PrefsGUI.RapidGUI;
#endif

#if PrefsGUI_RosettaUI
using RosettaUI;
using PrefsGUI.RosettaUI;
#endif

namespace PrefsGUI.Example
{
    public class PrefsGUIExample_Part3 : MonoBehaviour
#if PrefsGUI_RapidGUI
        ,IDoGUI 
#endif
#if PrefsGUI_RosettaUI
        ,IElementCreator
#endif
    {
        // define PrefsParams with key.
        public PrefsMinMaxInt prefsMinMaxInt = new("PrefsMinMaxInt");
        public PrefsMinMaxFloat prefsMinMaxFloat = new("PrefsMinMaxFloat");
        public PrefsMinMaxVector2 prefsMinMaxVector2 = new("PrefsMinMaxVector2");
        public PrefsMinMaxVector3 prefsMinMaxVector3 = new("PrefsMinMaxVector3");
        public PrefsMinMaxVector4 prefsMinMaxVector4 = new("PrefsMinMaxVector4");
        public PrefsMinMaxVector2Int prefsMinMaxVector2Int = new("PrefsMinMaxVector2Int");
        public PrefsMinMaxVector3Int prefsMinMaxVector3Int = new("PrefsMinMaxVector3Int");

#if PrefsGUI_RapidGUI
        public void DoGUI()
        {
            prefsMinMaxInt.DoGUISlider();
            prefsMinMaxFloat.DoGUISlider();
            prefsMinMaxVector2.DoGUISlider();
            prefsMinMaxVector3.DoGUISlider();
            prefsMinMaxVector4.DoGUISlider();
            prefsMinMaxVector2Int.DoGUISlider();
            prefsMinMaxVector3Int.DoGUISlider();
        }
#endif

#if PrefsGUI_RosettaUI
        public Element CreateElement(LabelElement _)
        {
            return UI.Column(
                prefsMinMaxInt.CreateMinMaxSlider(),
                prefsMinMaxFloat.CreateMinMaxSlider(),
                prefsMinMaxVector2.CreateMinMaxSlider(),
                prefsMinMaxVector3.CreateMinMaxSlider(),
                prefsMinMaxVector4.CreateMinMaxSlider(),
                prefsMinMaxVector2Int.CreateMinMaxSlider(),
                prefsMinMaxVector3Int.CreateMinMaxSlider()
            );
        }
#endif
    }
}