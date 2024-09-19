using System;
using System.Buffers;
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
    public class DividedSpawnDataReceiverSyncObject : SyncObject
    {
        public readonly SyncObject baseSyncObject;
        private readonly List<ArraySegment<byte>> _spawnData = new();
        
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
        }

        public override void Reset()
        {
        }
        
        #endregion


        public void AddDividedSpawnData(ArraySegment<byte> bytes) => _spawnData.Add(bytes);

        public void ApplyStockData()
        {
            ApplySpawnData();
            ApplyDeltaData();
        }

        private void ApplySpawnData()
        {
            var size = _spawnData.Sum(x => x.Count);
            
            var byteArray = ArrayPool<byte>.Shared.Rent(size);
            {
                var position = 0;
                foreach (var arraySegment in _spawnData)
                {
                    arraySegment.CopyTo(byteArray, position);
                    position += arraySegment.Count;
                }

                using var reader = NetworkReaderPool.Get(new ArraySegment<byte>(byteArray, 0, size));

                baseSyncObject.OnDeserializeAll(reader);
            }
            ArrayPool<byte>.Shared.Return(byteArray);
        }
        
        private void ApplyDeltaData()
        {
            // throw new NotImplementedException();
        }
    }
}