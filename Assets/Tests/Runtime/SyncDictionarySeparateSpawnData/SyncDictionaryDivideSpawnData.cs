using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine.Pool;

/// <summary>
/// 巨大なデータを想定したSyncDictionary
/// 
/// SyncDictionaryはデータが大きいとSpawn時のメッセージが大きくなりMirrorの一度に送れる容量をオーバーしてエラーになってしまう
/// Spawn時に送るデータは小さくし、その後のデータは別途回数を分けて送ることでエラーを回避する
/// 
/// デメリット
/// - クライアント側ではSpawn時に最新の状態になっていない
/// - 新しいクライアントがサーバーに接続してきた際、既存のSpawn済みのクライアントにも小分けSpawnデータが届く
/// </summary>
public class SyncDictionaryDivideSpawnData<TKey, TValue> : SyncDictionary<TKey, TValue>
{
    public enum DeltaDataMode
    {
        DividedSpawn,
        Changes
    }
    
    public int elementCountForSeparateSpawn = 2;
    private HashSet<TKey> _needSendSpawnKeys;
    
    
    public bool IsSendingSpawnData => _needSendSpawnKeys?.Any() ?? false;

    public override void OnSerializeAll(NetworkWriter writer)
    {
        if (objects.Count < elementCountForSeparateSpawn)
        {
            base.OnSerializeAll(writer);
        }
        else
        {
            // OnDeserializeAll()で無視されるように、要素数と変更点をゼロで送信する
            
            // writer.WriteUInt((uint)objects.Count);
            // writer.WriteUInt((uint)changes.Count);
            writer.WriteUInt(0);
            writer.WriteUInt(0);

            _needSendSpawnKeys = objects.Keys.ToHashSet();
        }
    }

    public override void OnSerializeDelta(NetworkWriter writer)
    {
        var hasNeedSendSpawnKey = _needSendSpawnKeys?.Any() ?? false;

        writer.WriteUInt((uint)(hasNeedSendSpawnKey
            ? DeltaDataMode.DividedSpawn
            : DeltaDataMode.Changes
        ));
        
        if (!hasNeedSendSpawnKey)
        {
            base.OnSerializeDelta(writer);
        }
        else
        {
            SerializeDeltaDividedSpawn(writer);
        }
    }

    private void SerializeDeltaDividedSpawn(NetworkWriter writer)
    {
        using var _ = ListPool<TKey>.Get(out var sendKeys);
        for(var i=0; i<elementCountForSeparateSpawn || _needSendSpawnKeys.Count == 0; i++)
        {
            var key = _needSendSpawnKeys.First();
            _needSendSpawnKeys.Remove(key);
            if ( objects.ContainsKey(key))
            {
                sendKeys.Add(key);
            }
        }
        
        writer.WriteUInt((uint)sendKeys.Count);

        foreach (var key in sendKeys)
        {
            writer.Write(key);
            writer.Write(objects[key]);
        }
    }
    
    
    public override void OnDeserializeDelta(NetworkReader reader)
    {
        var mode = (DeltaDataMode)reader.ReadUInt();
        switch (mode)
        {
            case DeltaDataMode.DividedSpawn:
                DeserializeDeltaDividedSpawn(reader);
                break;
            
            case DeltaDataMode.Changes:
                base.OnDeserializeDelta(reader);
                break;
            
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void DeserializeDeltaDividedSpawn(NetworkReader reader)
    {
        var count = reader.ReadUInt();
        for (var i = 0; i < count; i++)
        {
            var key = reader.Read<TKey>();
            var value = reader.Read<TValue>();
            objects[key] = value;
        }
    }


    public override void ClearChanges()
    {
        // SpawnDataを全部送るまではchangesをクリアしない
        if (IsSendingSpawnData) return;
        base.ClearChanges();
    }
}
