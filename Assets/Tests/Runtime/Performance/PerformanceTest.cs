#if PrefsGUI_RosettaUI

using System.Collections.Generic;
using System.Linq;
using Mirror;
using PrefsGUI.RosettaUI;
using RosettaUI;
using UnityEngine;

namespace PrefsGUI.Sync.Example
{
    public class PerformanceTest : NetworkBehaviour
    {
        public RosettaUIRoot uiRoot;
        public Vector2 windowPosition;
        
        public int count;
        [SyncVar]public bool updatePrefsValues;
        [SyncVar]public int showPrefsIndex;
        public List<PrefsFloat> prefsFloats;
        public List<PrefsString> prefsStrings;

        private static List<string> _stringValues = new();
        
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
                
                foreach (var prefs in prefsStrings)
                {
                    prefs.Set(_stringValues[Random.Range(0,count)]);
                }

            }
        }

        void ResetPrefs()
        {
            prefsFloats = Enumerable.Range(0, count)
                .Select(i => new PrefsFloat(nameof(PrefsFloat) + i))
                .ToList();
            
            prefsStrings = Enumerable.Range(0, count)
                .Select(i => new PrefsString(nameof(PrefsString) + i))
                .ToList();

            if (_stringValues.Count <= count)
            {
                _stringValues = Enumerable.Range(0, count).Select(i => i.ToString()).ToList();
            }
        }

        public Element CreateElement()
        {
            return UI.Window(nameof(PerformanceTest),
                UI.Field(() => count).RegisterValueChangeCallback(ResetPrefs),
                UI.Field(() => updatePrefsValues, b => updatePrefsValues = b),
                UI.DynamicElementOnStatusChanged(
                    () => count,
                    max => UI.Slider(
                        () => showPrefsIndex,
                        i => showPrefsIndex = i,
                        max - 1
                        )
                ),
                UI.DynamicElementOnStatusChanged(
                    () => showPrefsIndex,
                    (idx) =>
                    {
                        if (0 <= idx && idx < prefsFloats.Count)
                        {
                            return UI.Column(
                                prefsFloats[idx].CreateElement(),
                                prefsStrings[idx].CreateElement()
                            );
                        }

                        return null;
                    })
            ).SetPosition(windowPosition);
        }
    }
}

#endif