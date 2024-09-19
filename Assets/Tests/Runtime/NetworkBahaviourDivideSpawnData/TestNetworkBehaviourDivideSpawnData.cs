using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Mirror;
using RosettaUI;
using UnityEngine;

namespace PrefsGUI.Sync.Example
{
    public class TestNetworkBehaviourDivideSpawnData : NetworkBehaviourDivideSpawnData
    {
        public int elementCount = 1000;

        public RosettaUIRoot rosettaUIRoot;

        public readonly SyncDictionary<string, string> syncDictionary = new();

        private List<SyncObject> _syncObjects;
        private string _changeValueKey = "0";
        private bool _changeValueRandom = true;

        protected override IEnumerable<SyncObject> DivideTargetSyncObjects => _syncObjects ??= new List<SyncObject>
        {
            syncDictionary
        };


        private void Start()
        {
            if (isServer)
            {
                foreach (var i in Enumerable.Range(0, elementCount))
                {
                    syncDictionary.Add(i.ToString(), Random.value.ToString(CultureInfo.InvariantCulture));
                }
            }

            rosettaUIRoot.Build(CreateElement());
        }

        private void Update()
        {
            if (isServer && _changeValueRandom)
            {
                syncDictionary[_changeValueKey] = Random.value.ToString(CultureInfo.InvariantCulture);
            }
        }


        private Element CreateElement()
        {
            var displayStartIndex = 0;
            var displayElementCount = 10;
            
            return UI.Window(
                UI.Page(
                    UI.Field(() => displayStartIndex),
                    UI.Field(() => displayElementCount),
                    UI.Field(() => _changeValueRandom),
                    UI.Field(() => _changeValueKey),
                    UI.FieldReadOnly("SyncDictionary", () =>
                        {
                            var elementStrings = syncDictionary
                                .Skip(displayStartIndex)
                                .Take(displayElementCount)
                                .Select(pair => pair.ToString());

                            return string.Join("\n", elementStrings);
                        }
                    )
                )
            ).SetPosition(new Vector2(300f, 0f)).SetWidth(500f);
        }
    }
}