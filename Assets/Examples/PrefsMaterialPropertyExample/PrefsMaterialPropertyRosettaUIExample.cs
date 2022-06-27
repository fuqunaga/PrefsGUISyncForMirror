using UnityEngine;

#if PrefsGUI_RosettaUI
using RosettaUI;
using PrefsGUI.RosettaUI;
#endif

namespace PrefsGUI.Example
{
#if PrefsGUI_RosettaUI
    [RequireComponent(typeof(RosettaUIRoot))]
    public class PrefsMaterialPropertyRosettaUIExample : MonoBehaviour
    {
        public PrefsMaterialProperty prefsMaterialProperty;
        public Vector2 position;
        
        public void Start()
        {
            var rosettaUIRoot = GetComponent<RosettaUIRoot>();
            
            rosettaUIRoot.Build(
                UI.Window(
                    prefsMaterialProperty.CreateElement()
                ).SetPosition(position)
            );
        }
    }
#else
    public class PrefsMaterialPropertyRosettaUIExample : MonoBehaviour
    {
    }
#endif
}