using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;

namespace PrefsGUI.Sync
{
    /// <summary>
    /// Spawnデータの分割対応したダミーのSyncObject
    ///
    /// 受信した分割Spawnデータとその間のDeltaデータを保存し、Spawnデータが全て揃った段階でまとめて本来のSyncObjectに反映する
    /// </summary>
    public class DividedSpawnDataReceiverSyncObject : SyncObject, IDisposable
    {
        public readonly SyncObject baseSyncObject;
        private readonly List<ArraySegmentPooled<byte>> _spawnDataList = new();
        private readonly List<ArraySegmentPooled<byte>> _deltaDataList = new();
        
        public DividedSpawnDataReceiverSyncObject(SyncObject baseSyncObject) => this.baseSyncObject = baseSyncObject;


        #region override SyncObject
        
        public override void ClearChanges()
        {
        }

        public override void OnSerializeAll(NetworkWriter writer)
        {
        }

        public override void OnSerializeDelta(NetworkWriter writer)
        {
        }

        public override void OnDeserializeAll(NetworkReader reader)
        {
        }

        public override void OnDeserializeDelta(NetworkReader reader)
        {
            // readerを進めるために一旦SyncObjectのOnDeserializeDeltaを呼ぶ
            // ここで適用された値は私用せずApplyStockDataで改めて適用する
            var startPosition = reader.Position;
            baseSyncObject.OnDeserializeDelta(reader);
            
            var size = reader.Position - startPosition;
            reader.Position = startPosition;
            var bytes = reader.ReadBytesSegment(size);
            
            AddDeltaData(bytes);
        }

        public override void Reset()
        {
        }
        
        #endregion


        public void AddDividedSpawnData(ArraySegment<byte> bytesVolatile)
        {   
            // このbytesVolatileはNetworkReaderの参照なのでいずれ上書きされるっぽい
            // コピーして保存する
            var bytes = ArraySegmentPooled<byte>.Get(bytesVolatile.Count);
            bytesVolatile.CopyTo(bytes);
            _spawnDataList.Add(bytes);
        }

        private void AddDeltaData(ArraySegment<byte> bytesVolatile)
        {
            // このbytesVolatileはNetworkReaderの参照なのでいずれ上書きされるはず
            // コピーして保存する
            var bytes = ArraySegmentPooled<byte>.Get(bytesVolatile.Count);
            bytesVolatile.CopyTo(bytes);

            _deltaDataList.Add(bytes);
        }

        public void ApplyStockData()
        {
            baseSyncObject.Reset();
            ApplySpawnData();
            ApplyDeltaData();
        }

        private void ApplySpawnData()
        {
            var size = _spawnDataList.Sum(x => x.ArraySegment.Count);
            using var totalBytes = ArraySegmentPooled<byte>.Get(size);
            
            var position = 0;
            foreach (var arraySegmentPooled in _spawnDataList)
            {
                var arraySegment = arraySegmentPooled.ArraySegment;
                arraySegment.CopyTo(totalBytes.ArraySegment.Slice(position, arraySegment.Count));
                position += arraySegment.Count;
                
                arraySegmentPooled.Dispose();
            }
            _spawnDataList.Clear();

            using var reader = NetworkReaderPool.Get(totalBytes);
            baseSyncObject.OnDeserializeAll(reader);
        }
        
        private void ApplyDeltaData()
        {
            foreach (var deltaData in _deltaDataList)
            {
                using var reader = NetworkReaderPool.Get(deltaData);
                baseSyncObject.OnDeserializeDelta(reader);
                
                deltaData.Dispose();
            }
            _deltaDataList.Clear();
        }

        public void Dispose()
        {
            foreach (var arraySegmentPooled in _spawnDataList)
            {
                arraySegmentPooled.Dispose();
            }
            _spawnDataList.Clear();
            
            foreach (var arraySegmentPooled in _deltaDataList)
            {
                arraySegmentPooled.Dispose();
            }
            _deltaDataList.Clear();
        }
    }
}