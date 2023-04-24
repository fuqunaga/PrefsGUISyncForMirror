using System;
using Mirror;
using UnityEngine;

namespace PrefsGUI.Sync
{
    public static partial class BytesConverter
    {
        static BytesConverter()
        {
            RegisterWriterReaderRectInt();
            RegisterWriterReaderRectOffset();
            RegisterWriterReaderBounds();
            RegisterWriterReaderBoundsInt();
        }

        private static void RegisterWriterReaderRectInt()
        {
            RegisterWriterReader(
                (writer, value) =>
                {
                    writer.Write(value.x);
                    writer.Write(value.y);
                    writer.Write(value.width);
                    writer.Write(value.height);
                },
                (reader) => new RectInt
                {
                    x = reader.ReadInt(),
                    y = reader.ReadInt(),
                    width = reader.ReadInt(),
                    height = reader.ReadInt()
                });
        }

        
        private static void RegisterWriterReaderRectOffset()
        {
            RegisterWriterReader(
                (writer, value) =>
                {
                    writer.Write(value.left);
                    writer.Write(value.right);
                    writer.Write(value.top);
                    writer.Write(value.bottom);
                },
                (reader) => new RectOffset
                {
                    left = reader.ReadInt(),
                    right = reader.ReadInt(),
                    top = reader.ReadInt(),
                    bottom = reader.ReadInt()
                });
        }

        
        private static void RegisterWriterReaderBounds()
        {
            RegisterWriterReader(
                (writer, bounds) =>
                {
                    writer.Write(bounds.center);
                    writer.Write(bounds.extents);
                },
                (reader) => new Bounds
                {
                    center = reader.ReadVector3(),
                    extents = reader.ReadVector3()
                });
        }
        
        private static void RegisterWriterReaderBoundsInt()
        {
            RegisterWriterReader(
                (writer, bounds) =>
                {
                    writer.Write(bounds.position);
                    writer.Write(bounds.size);
                },
                (reader) => new BoundsInt
                {
                    position = reader.ReadVector3Int(),
                    size = reader.ReadVector3Int()
                });
        }


        
        

        private static void RegisterWriterReader<T>(Action<NetworkWriter, T> writeFunc, Func<NetworkReader, T> readFunc)
        {
            Writer<T>.write ??= writeFunc;
            Reader<T>.read ??= readFunc;
        }
    }
}