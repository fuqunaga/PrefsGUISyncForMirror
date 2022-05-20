using System.Collections.Generic;
using System.Linq;
using RosettaUI;
using UnityEngine;

namespace PrefsGUI.Sync.Example
{
    public class PerformanceTest : MonoBehaviour
    {
        public RosettaUIRoot uiRoot;
        public Vector2 windowPosition;
        
        public int count;
        public bool updateValue;
        public List<PrefsFloat> prefsFloats;

        public void Start()
        {
            ResetPrefs();
            
            uiRoot.Build(CreateElement());
        }

        private void Update()
        {
            if (updateValue)
            {
                foreach (var prefs in prefsFloats)
                {
                    prefs.Set(Random.value);
                }
            }
        }

        void ResetPrefs()
        {
            prefsFloats = Enumerable.Range(0, count)
                .Select(i => new PrefsFloat(nameof(PrefsFloat) + i))
                .ToList();
        }

        public Element CreateElement()
        {
            return UI.Window(nameof(PerformanceTest),
                UI.Field(() => count).RegisterValueChangeCallback(ResetPrefs),
                UI.Field(() => updateValue)
            ).SetWidth(500f).SetPosition(windowPosition);
        }
    }
}