using System;
using System.Collections.Generic;
using System.Reflection;
using Mirror;

namespace PrefsGUI.Sync
{
    public static class BytesConverter
    {
        #region ToBytes
        
        
        private static readonly MethodInfo ToBytesMethodInfo = typeof(BytesConverter).GetMethod(nameof(_ObjToBytes), BindingFlags.Static | BindingFlags.NonPublic);
        private static readonly Dictionary<Type, MethodInfo> TypeToToBytesMethodInfo = new();
        private static readonly object[] ToBytesArgs = new object[2];

        public static void ObjToBytes(Type type, object obj, ref byte[] bytes)
        {
            if (!TypeToToBytesMethodInfo.TryGetValue(type, out var mi))
            {
                mi = ToBytesMethodInfo.MakeGenericMethod(type);
                TypeToToBytesMethodInfo[type] = mi;
            }

            ToBytesArgs[0] = obj;
            ToBytesArgs[1] = bytes;

            mi.Invoke(null, ToBytesArgs);

            bytes = (byte[])ToBytesArgs[1];
        }
        
        
        private static void _ObjToBytes<T>(object obj, ref byte[] bytes)
        {
            using var writer = NetworkWriterPool.Get();
            writer.Write((T)obj);

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
        
        #endregion
        
        
        #region ToObj
                
        private static readonly MethodInfo ToObjMethodInfo =
            typeof(BytesConverter).GetMethod(nameof(_BytesToObj), BindingFlags.Static | BindingFlags.NonPublic);

        private static readonly Dictionary<Type, MethodInfo> TypeToToObjMethodInfo = new();
        private static readonly object[] ToObjArg = new object[1];


        public static object BytesToObj(Type type, byte[] bytes)
        {
            if (!TypeToToObjMethodInfo.TryGetValue(type, out var mi))
            {
                mi = ToObjMethodInfo.MakeGenericMethod(type);
                TypeToToObjMethodInfo[type] = mi;
            }

            ToObjArg[0] = bytes;
            return mi.Invoke(null, ToObjArg);
        }

        private static T _BytesToObj<T>(byte[] bytes)
        {
            using var reader = NetworkReaderPool.Get(bytes);
            return reader.Read<T>();
        }
        
        #endregion
    }
}