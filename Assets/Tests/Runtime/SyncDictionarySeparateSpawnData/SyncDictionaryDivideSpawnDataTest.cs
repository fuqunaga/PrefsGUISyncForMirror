using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using RosettaUI;

public class SyncDictionaryDivideSpawnDataTest : NetworkBehaviour
{
    [Serializable]
    public class KeyValue
    {
        public string key;
        public string value;
    }
    
    public readonly SyncDictionaryDivideSpawnData<string, string> syncDictionary = new();
    
    
    public RosettaUIRoot rosettaUIRoot;
    public List<KeyValue> defaultKeyValueList = new();

    private void Start()
    {
        if (!syncDictionary.IsReadOnly)
        {
            foreach (var kv in defaultKeyValueList)
            {
                syncDictionary.TryAdd(kv.key, kv.value);
            }
        }

        rosettaUIRoot.Build(CreateElement());
    }

    private Element CreateElement()
    {
        var key = "";
        var value = "";

        return UI.Window(
            nameof(SyncDictionaryDivideSpawnDataTest),

            UI.Field("SyncDictionaryDivideSpawnData",
                () => string.Join('\n', syncDictionary.Select(kv => $" {kv.Key}: {kv.Value}"))),
            UI.Row(
                UI.Field(() => key),
                UI.Field(() => value)
            ),
            UI.Button("Add", () => syncDictionary.Add(key, value)),
            UI.Button("Remove", () =>
                {
                    if (!syncDictionary.Any()) return;
                    syncDictionary.Remove(syncDictionary.Keys.Last());
                }
            )
        );
    }
}
