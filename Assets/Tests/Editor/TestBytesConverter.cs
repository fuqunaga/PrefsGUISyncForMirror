using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Xml.Serialization;
using NUnit.Framework;
using UnityEngine;

namespace PrefsGUI.Sync.Tests.Editor
{
    public class TestBytesConverter
    {
        
        [TestCaseSource(nameof(GetCsharpPrimitives), Category = "C# Primitives")]
        [TestCaseSource(nameof(GetUnityPrimitives), Category = "Unity Primitives")]
        public void TestPrimitives<T>(T value)
        {
            var bytes = Array.Empty<byte>();
            BytesConverter.ValueToBytes(value, ref bytes);
            var valueConverted = BytesConverter.BytesToValue<T>(bytes);

            if (typeof(T).IsValueType)
            {
                Assert.AreEqual(value, valueConverted);
            }
            else
            {
                var fields = typeof(T).GetFields();
                var properties = typeof(T).GetProperties();

                foreach (var field in fields)
                {
                    var expected = field.GetValue(value);
                    var actual = field.GetValue(valueConverted);
                    
                    Assert.AreEqual(expected, actual, $"field: {field.Name}, expected: {expected}, actual: {actual}");
                }
                
                foreach (var property in properties)
                {
                    var expected = property.GetValue(value);
                    var actual = property.GetValue(valueConverted);
                    
                    Assert.AreEqual(expected, actual, $"property: {property.Name}, expected: {expected}, actual: {actual}");
                }
            }
        }

        private static TestCaseData[] GetCsharpPrimitives()
        {
            return new TestCaseData[]
            {
                new(true),
                new((byte)1),
                new((sbyte)1),
                new((char)1),
                new((double)1d),
                new((float)1f),
                new((int)1),
                new((uint)1),
                new((long)1),
                new((ulong)1),
                new((short)1),
                new((ushort)1),
            };
        }
        
        private static TestCaseData[] GetUnityPrimitives()
        {
            return new TestCaseData[]
            {
                new(Vector2.one),
                new(Vector2Int.one),
                new(Vector3.one),
                new(Vector3Int.one),
                new(Vector4.one),
                new(Quaternion.identity),
                new(Color.white),
                new(new Color32(1, 1, 1, 1)),
                new(new Rect(1f, 1f, 1f, 1f)),
                new(new RectInt(1, 1, 1, 1)),
                new(new RectOffset(1, 1, 1, 1)),
                new(Matrix4x4.identity),
                new(new Plane(Vector3.one, 1f)),
                new(new Bounds(Vector3.one, Vector3.one)),
                new(new BoundsInt(Vector3Int.one, Vector3Int.one))
            };
        }
    }
}