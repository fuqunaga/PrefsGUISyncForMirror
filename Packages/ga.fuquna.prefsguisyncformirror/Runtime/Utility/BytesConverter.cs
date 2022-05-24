using System;
using System.Collections.Generic;
using System.Reflection;
using Mirror;
using UnityEngine;

namespace PrefsGUI.Sync
{
    public static class BytesConverter
    {
        public static void ValueToBytes<T>(T value, ref byte[] bytes)
        {
            using var writer = NetworkWriterPool.Get();
            if (Writer<T>.write != null)
            {
                writer.Write(value);
            }
            else
            {
                var str = JsonUtility.ToJson(value);
                writer.Write(str);
            }

            var seg = writer.ToArraySegment();
            if (bytes != null && bytes.Length == seg.Count)
            {
                seg.CopyTo(bytes);
            }
            else
            {
                bytes = seg.ToArray();
            }
        }
        
        
        public static T BytesToValue<T>(byte[] bytes)
        {
            using var reader = NetworkReaderPool.Get(bytes);
            if (Reader<T>.read != null)
            {
                return reader.Read<T>();
            }
            else
            {
                var str = reader.Read<string>();
                return JsonUtility.FromJson<T>(str);
            }
        }
    }
}