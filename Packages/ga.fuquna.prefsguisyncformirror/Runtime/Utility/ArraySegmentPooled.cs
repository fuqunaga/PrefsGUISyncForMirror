using System;
using System.Buffers;

namespace PrefsGUI.Sync
{
    /// <summary>
    /// ArrayPoolを使ってArraySegmentをプールする構造体
    /// </summary>
    public struct ArraySegmentPooled<T> : IDisposable
    {
        public static ArraySegmentPooled<T> Get(int size)
        {
            var array = ArrayPool<T>.Shared.Rent(size);
            var arraySegment = new ArraySegment<T>(array, 0, size);

            return new ArraySegmentPooled<T>()
            {
                ArraySegment = arraySegment
            };
        }
            
        public static implicit operator ArraySegment<T> (ArraySegmentPooled<T> arraySegmentPooled) => arraySegmentPooled.ArraySegment;
            
        public ArraySegment<T> ArraySegment { get; private set; }


        public void Dispose()
        {
            if (ArraySegment.Array != null)
            {
                ArrayPool<T>.Shared.Return(ArraySegment.Array);
            }
        }
    }
}