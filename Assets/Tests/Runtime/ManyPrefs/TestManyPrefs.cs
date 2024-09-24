#if PrefsGUI_RosettaUI

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Mirror;
using PrefsGUI.RosettaUI;
using RosettaUI;
using UnityEngine;

namespace PrefsGUI.Sync.Example
{
    /// <summary>
    /// 大量のPrefsでのSpawn時にエラーがでないかテスト
    /// </summary>
    public class TestManyPrefs : MonoBehaviour
    {
        public RosettaUIRoot uiRoot;
        public Vector2 windowPosition;
        
        public int count;
        
        public int showPrefsIndex;
        public int showPrefsCount;
        public bool updatePrefsValue;
        public bool setRandomValueToAllPrefs;
        public List<PrefsString> prefsStrings;

        private static List<string> _stringValues = new();
        
        public void Start()
        {
            ResetPrefs();
            if (setRandomValueToAllPrefs)
            {
                SetRandomValueToAllPrefs();
            }
            
            uiRoot.Build(CreateElement());
        }
        
        private void Update()
        {
            if (!updatePrefsValue) return;

            if (showPrefsIndex <= _stringValues.Count)
                prefsStrings[showPrefsIndex].Set(Random.value.ToString(CultureInfo.InvariantCulture));
        }


        private void ResetPrefs()
        {
            prefsStrings = Enumerable.Range(0, count)
                .Select(i => new PrefsString(nameof(PrefsString) + i, i.ToString(CultureInfo.InvariantCulture)))
                .ToList();

            if (_stringValues.Count <= count)
            {
                _stringValues = Enumerable.Range(0, count).Select(i => i.ToString()).ToList();
            }
        }
        
        private void SetRandomValueToAllPrefs()
        {
            foreach (var prefsString in prefsStrings)
            {
                prefsString.Set(Random.value.ToString(CultureInfo.InvariantCulture));
            }
        }

        private Element CreateElement()
        {
            return UI.Window(nameof(PerformanceTest),
                UI.Field(() => count).RegisterValueChangeCallback(ResetPrefs),
                UI.Field(() => updatePrefsValue),
                UI.Field(() => showPrefsCount),
                UI.DynamicElementOnStatusChanged(
                    () => count,
                    max => UI.Slider(
                        () => showPrefsIndex,
                        i => showPrefsIndex = i,
                        max - 1
                        )
                ),
                UI.DynamicElementOnStatusChanged(
                    () => (showPrefsIndex, showPrefsCount),
                    _ =>
                    {
                        return UI.Column(
                            prefsStrings
                                .Skip(showPrefsIndex)
                                .Take(showPrefsCount)
                                .Select(prefs => prefs.CreateElement())
                        );
                    })
            ).SetPosition(windowPosition);
        }
    }
}

#endif
