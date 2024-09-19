using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Pool;

namespace PrefsGUI.Sync
{
    /// <summary>
    /// Spawn時のデータが巨大になる場合に分割して送信するNetworkBehaviour
    /// </summary>
    public abstract class NetworkBehaviourDivideSpawnData : NetworkBehaviour
    {
        #region Server parameters
        
        public int spawnDataSizePerChunk = 100 * 1024;
        
        // すでに分割Spawnを開始したコネクションのリスト
        private readonly HashSet<NetworkConnectionToClient> _spawnedConnections = new();
        
        #endregion
        
        
        #region Client paramters
        
        private int _lastReceivedSyncObjectIndex = -1;
        
        #endregion
        
        // Spawnデータを分割するSyncObjectのリスト
        // サーバーとクライアントで同じものを指定している必要がある
        // SerializeAll/DeserializeAllでは無視しTargetRpcで別途データを分割送信する
        protected abstract IEnumerable<SyncObject> DivideTargetSyncObjects { get; }

        // すべてのSpawn処理が終わったか
        // クライアント側のOnDeserializeでfalseになる
        public virtual bool IsSpawnFinished { get; protected set; } = true;


        private void CheckNewConnection(ulong divideTargetMask)
        {
            if (divideTargetMask == 0) return;

            Assert.IsTrue(syncDirection == SyncDirection.ServerToClient);
            
            using var _ = ListPool<NetworkConnectionToClient>.Get(out var newConnections);
            newConnections.AddRange(netIdentity.observers.Values
                .Where(connection => connection is not LocalConnectionToClient)
                .Except(_spawnedConnections));

            if (newConnections.Count == 0) return;

            foreach (var connection in newConnections)
            {
                StartSendSpawnData(connection, divideTargetMask);
                _spawnedConnections.Add(connection);
            }
        }

        private void StartSendSpawnData(NetworkConnectionToClient connection, ulong divideTargetMask)
        {
            StartCoroutine(SendSpawnDataCoroutine(connection, divideTargetMask));
        }

        private IEnumerator SendSpawnDataCoroutine(NetworkConnectionToClient connection, ulong divideTargetMask)
        {
            for (var syncObjectIndex = 0; syncObjectIndex < syncObjects.Count; ++syncObjectIndex)
            {
                if ((divideTargetMask & (1ul << syncObjectIndex)) == 0) continue;
                var targetSyncObject = syncObjects[syncObjectIndex];
                
                
                // すべてをNetworkWriterに書き出してから分割して送信
                using var writer = NetworkWriterPool.Get();
                targetSyncObject.OnSerializeAll(writer);
                
                var bytes = writer.ToArraySegment();
                for(var sendPosition = 0; sendPosition < bytes.Count; sendPosition += spawnDataSizePerChunk)
                {
                    // 即送信はダメ
                    // NetworkBehaviour.OnSerialize()内で呼ばれるがそこでは送信できないっぽい
                    yield return null;
                    
                    var lastSend = sendPosition + spawnDataSizePerChunk >= bytes.Count;
                    var sendSize = lastSend ? bytes.Count - sendPosition : spawnDataSizePerChunk;

                    TargetSendDividedSpawnData(connection,
                        syncObjectIndex,
                        bytes.Slice(sendPosition, sendSize),
                        lastSend
                    );
                }
            }
        }
        
        [TargetRpc]
        private void TargetSendDividedSpawnData(NetworkConnectionToClient target, int syncObjectIndex, ArraySegment<byte> bytes, bool isLast)
        {
            if ( _lastReceivedSyncObjectIndex >= 0 && _lastReceivedSyncObjectIndex != syncObjectIndex)
            {
                Debug.LogWarning($"Received SyncObjectIndex is not match. Expected: {_lastReceivedSyncObjectIndex}, Received: {syncObjectIndex}");
                return;
            }

            _lastReceivedSyncObjectIndex = syncObjectIndex;

            if (syncObjects[syncObjectIndex] is not DividedSpawnDataReceiverSyncObject receiverSyncObject)
            {
                Debug.LogWarning($"SyncObject is not DummySyncObjectDivideSpawnData. Index: {syncObjectIndex}");
                return;
            }
            
            receiverSyncObject.AddDividedSpawnData(bytes);
            if (isLast)
            {
                receiverSyncObject.ApplyStockData();
                syncObjects[syncObjectIndex] = receiverSyncObject.baseSyncObject;
                
                // すべてのSyncObjectが終わったか確認
                if (syncObjects.All(syncObject => syncObject is not DividedSpawnDataReceiverSyncObject))
                {
                    IsSpawnFinished = true;
                }
            }
        }

        
        #region override OnSerialize/OnDeserialize
        
        public override void OnSerialize(NetworkWriter writer, bool initialState)
        {
            // Debug.Log($"{nameof(OnSerialize)}: {initialState}, {Time.frameCount}");
            
            if (!initialState)
            {
                base.OnSerialize(writer, false);
                // SerializeSpawningDeltaData();
            }
            else
            {
                var divideTargetMask = DivideTargetSyncObjects
                    .Aggregate(0ul, (current, divideTargetSyncObject) => current | 1ul << syncObjects.IndexOf(divideTargetSyncObject));
        
                SerializeObjectsAllWithMask(writer, divideTargetMask);
                SerializeSyncVars(writer, true);
        
                CheckNewConnection(divideTargetMask);
            }
        }
  
        private void SerializeObjectsAllWithMask(NetworkWriter writer, ulong divideTargetMask)
        {
             Compression.CompressVarUInt(writer, divideTargetMask);
             
             for(var i=0; i<syncObjects.Count; i++)
             {
                 if ((divideTargetMask & (1ul << i)) == 0)
                 {
                     syncObjects[i].OnSerializeAll(writer);
                 }
             }
        }

        
        public override void OnDeserialize(NetworkReader reader, bool initialState)
        {
            // Debug.Log($"{nameof(OnDeserialize)}: {initialState}, {Time.frameCount}");
            
            if (!initialState)
            {
                base.OnDeserialize(reader, false);
            }
            else
            {
                DeserializeObjectsAllWithMask(reader);
                DeserializeSyncVars(reader, true);
            }
        }
        
        private void DeserializeObjectsAllWithMask(NetworkReader reader)
        {
            Debug.Log($"{nameof(OnDeserialize)}: {Time.frameCount}");
            
            var divideTargetMask = Compression.DecompressVarUInt(reader);

            for (var i = 0; i < syncObjects.Count; i++)
            {
                if ((divideTargetMask & (1ul << i)) == 0)
                {
                    syncObjects[i].OnDeserializeAll(reader);
                }
                // DivideTargetはダミーに入れ替える
                else
                {
                    syncObjects[i] = new DividedSpawnDataReceiverSyncObject(syncObjects[i]); 
                }
            }

            if (divideTargetMask != 0)
            {
                IsSpawnFinished = false;
            }
        }
        
        #endregion
    }
}