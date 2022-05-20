using System.Collections.Generic;
using System.Linq;
using Mirror;
using PrefsGUI.RosettaUI;
using RosettaUI;
using UnityEngine;
using UnityEngine.Serialization;

namespace PrefsGUI.Sync.Example
{
    public class PerformanceTest : NetworkBehaviour
    {
        public RosettaUIRoot uiRoot;
        public Vector2 windowPosition;
        
        public int count;
        [FormerlySerializedAs("updateValue")] public bool updatePrefsValues;
        public List<PrefsFloat> prefsFloats;

        public int showPrefsIndex;

        public void Start()
        {
            ResetPrefs();
            
            uiRoot.Build(CreateElement());
        }

        private void Update()
        {
            if (updatePrefsValues)
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
                UI.Field(() => updatePrefsValues),
                UI.DynamicElementOnStatusChanged(
                    () => count,
                    max => UI.Slider(() => showPrefsIndex, max-1).RegisterValueChangeCallback(() => OnShowPrefsIndexChanged(showPrefsIndex))
                ),
                UI.DynamicElementOnStatusChanged(
                    () => showPrefsIndex,
                    (idx) =>  (0<=idx && idx <prefsFloats.Count) ? prefsFloats[idx].CreateElement() : null
                    )
            ).SetPosition(windowPosition);
        }

        [ClientRpc]
        private void OnShowPrefsIndexChanged(int index)
        {
            showPrefsIndex = index;
        }
    }
}