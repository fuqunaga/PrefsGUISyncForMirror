using UnityEngine;
#if PrefsGUI_RapidGUI
using PrefsGUI.RapidGUI;
using UnityEngine.Serialization;
#endif

namespace PrefsGUI.Example
{
#if PrefsGUI_RapidGUI
    public class PrefsMaterialPropertyRapidGUIExample : PrefsGUIRapidGUIExampleBase
    {
        [FormerlySerializedAs("debugMenu")] public PrefsMaterialProperty prefsMaterialProperty;

        public void Start()
        {
            if (prefsMaterialProperty == null)
            {
                prefsMaterialProperty = GetComponent<PrefsMaterialProperty>();
            }
        }

        protected override void DoGUI()
        {
            if (prefsMaterialProperty != null)
            {
                prefsMaterialProperty.DoGUI();
            }

            base.DoGUI();
        }
    }
#else
    public class PrefsMaterialPropertyRapidGUIExample : MonoBehaviour
    {}
#endif
}