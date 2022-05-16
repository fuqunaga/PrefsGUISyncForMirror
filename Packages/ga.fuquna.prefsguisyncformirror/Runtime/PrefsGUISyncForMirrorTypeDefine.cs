using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace PrefsGUI.Sync
{
    public partial class PrefsGUISyncForMirror : NetworkBehaviour
    {
        #region KeyObj

        public interface IKeyObj
        {
            string Key { get; set; }
            object Obj { get; set; }
            Type ValueType { get; }
        }


        public struct KeyBool : IKeyObj
        {
            public string key;
            public bool value;
            public string Key { get => key; set => key = value; }
            public object Obj { get => value; set => this.value = (bool)value; }
            public Type ValueType => typeof(bool);
        }

        public struct KeyInt : IKeyObj
        {
            public string key;
            public int value;
            public string Key { get => key; set => key = value; }
            public object Obj { get => value; set => this.value = (int)value; }
            public Type ValueType => typeof(int);
        }

        public struct KeyUInt : IKeyObj
        {
            public string key;
            public uint value;
            public string Key { get => key; set => key = value; }
            public object Obj { get => value; set => this.value = (uint)value; }
            public Type ValueType => typeof(uint);
        }

        public struct KeyFloat : IKeyObj
        {
            public string key;
            public float value;
            public string Key { get => key; set => key = value; }
            public object Obj { get => value; set => this.value = (float)value; }
            public Type ValueType => typeof(float);
        }

        public struct KeyString : IKeyObj
        {
            public string key;
            public string value;
            public string Key { get => key; set => key = value; }
            public object Obj { get => value; set => this.value = (string)value; }
            public Type ValueType => typeof(string);
        }

        public struct KeyColor : IKeyObj
        {
            public string key;
            public Color value;
            public string Key { get => key; set => key = value; }
            public object Obj { get => value; set => this.value = (Color)value; }
            public Type ValueType => typeof(Color);
        }

        public struct KeyVector2 : IKeyObj
        {
            public string key;
            public Vector2 value;
            public string Key { get => key; set => key = value; }
            public object Obj { get => value; set => this.value = (Vector2)value; }
            public Type ValueType => typeof(Vector2);
        }

        public struct KeyVector3 : IKeyObj
        {
            public string key;
            public Vector3 value;
            public string Key { get => key; set => key = value; }
            public object Obj { get => value; set => this.value = (Vector3)value; }
            public Type ValueType => typeof(Vector3);
        }

        public struct KeyVector4 : IKeyObj
        {
            public string key;
            public Vector4 value;
            public string Key { get => key; set => key = value; }
            public object Obj { get => value; set => this.value = (Vector4)value; }
            public Type ValueType => typeof(Vector4);
        }

        // UNET doesn't support Vector2Int
        public struct KeyVector2Int : IKeyObj
        {
            public string key;
            public Vector2Int_ value;
            public string Key { get => key; set => key = value; }
            public object Obj { get => (Vector2Int)value; set => this.value = (Vector2Int)value; }
            public Type ValueType => typeof(Vector2Int);
        }

        
        public struct KeyVector3Int : IKeyObj
        {
            public string key;
            public Vector3Int_ value;
            public string Key { get => key; set => key = value; }
            public object Obj { get => (Vector3Int)value; set => this.value = (Vector3Int)value; }
            public Type ValueType => typeof(Vector3Int);
        }

        public struct KeyRect : IKeyObj
        {
            public string key;
            public Rect value;
            public string Key { get => key; set => key = value; }
            public object Obj { get => value; set => this.value = (Rect)value; }
            public Type ValueType => typeof(Rect);
        }

        public struct KeyBounds : IKeyObj
        {
            public string key;
            public Bounds_ value;
            public string Key { get => key; set => key = value; }
            public object Obj { get => (Bounds)value; set => this.value = (Bounds)value; }
            public Type ValueType => typeof(Bounds);
        }

        public struct KeyBoundsInt : IKeyObj
        {
            public string key;
            public BoundsInt_ value;
            public string Key { get => key; set => key = value; }
            public object Obj { get => (BoundsInt)value; set => this.value = (BoundsInt)value; }
            public Type ValueType => typeof(BoundsInt);
        }


        #region Substitute for strcut that out of UNet support 

        public struct Vector2Int_
        {
            public int x, y;

            public static implicit operator Vector2Int_(Vector2Int v) => new Vector2Int_() { x = v.x, y = v.y };
            public static implicit operator Vector2Int(Vector2Int_ v) => new Vector2Int(v.x, v.y);
        }

        public struct Vector3Int_
        {
            public int x, y, z;

            public static implicit operator Vector3Int_(Vector3Int v) => new Vector3Int_() { x = v.x, y = v.y, z = v.z };
            public static implicit operator Vector3Int(Vector3Int_ v) => new Vector3Int(v.x, v.y, v.z);
        }

        public struct Bounds_
        {
            public Vector3 center, size;

            public static implicit operator Bounds_(Bounds v) => new Bounds_() { center = v.center, size = v.size };
            public static implicit operator Bounds(Bounds_ v) => new Bounds() { center = v.center, size = v.size };
        }

        public struct BoundsInt_
        {
            public Vector3Int_ position, size;

            public static implicit operator BoundsInt_(BoundsInt v) => new BoundsInt_() { position = v.position, size = v.size };
            public static implicit operator BoundsInt(BoundsInt_ v) => new BoundsInt() { position = v.position, size = v.size };
        }

        #endregion

        #endregion


        #region SyncListKey*

        public class SyncListKeyBool : SyncList<KeyBool> { }

        public class SyncListKeyInt : SyncList<KeyInt> { }

        public class SyncListKeyUInt : SyncList<KeyUInt> { }

        public class SyncListKeyFloat : SyncList<KeyFloat> { }

        public class SyncListKeyString : SyncList<KeyString> { }

        public class SyncListKeyColor : SyncList<KeyColor> { }

        public class SyncListKeyVector2 : SyncList<KeyVector2> { }

        public class SyncListKeyVector3 : SyncList<KeyVector3> { }

        public class SyncListKeyVector4 : SyncList<KeyVector4> { }

        public class SyncListKeyVector2Int : SyncList<KeyVector2Int> { }

        public class SyncListKeyVector3Int : SyncList<KeyVector3Int> { }

        public class SyncListKeyRect : SyncList<KeyRect> { }

        public class SyncListKeyBounds : SyncList<KeyBounds> { }

        public class SyncListKeyBoundsInt : SyncList<KeyBoundsInt> { }

        #endregion


        public interface ISyncListKeyObj
        {
            (string key, object obj) Get(int i);
            void Set(int i, string key, object obj);
            void Add(string key, object obj);
            int Count { get; }
            Type ValueType { get; }
        }

        public static class SyncListKeyObjHelper
        {
            public static SyncListKeyObj<T> Create<T>(SyncList<T> sl)
                where T : struct, IKeyObj
            {
                return new SyncListKeyObj<T>(sl);
            }
        }

        public class SyncListKeyObj<TKeyObj> : ISyncListKeyObj
            where TKeyObj : struct, IKeyObj
        {
            SyncList<TKeyObj> sl;
            Dictionary<int, object> cachedObj = new Dictionary<int, object>();


            public SyncListKeyObj(SyncList<TKeyObj> sl)
            {
                this.sl = sl;
                sl.Callback += (op, itemIndex, _, _) => RemoveCache(itemIndex);
            }

            void RemoveCache(int i)
            {
                cachedObj.Remove(i);
            }


            #region ISyncListKeyObj

            public (string key, object obj) Get(int i)
            {
                var kv = sl[i];
                if  (!cachedObj.TryGetValue(i, out var obj))
                {
                    cachedObj[i] = obj = kv.Obj;
                }

                return (kv.Key, obj);
            }

            public void Set(int i, string key, object obj)
            {
                var (_, old) = Get(i);
                if (!old.Equals(obj))
                {
                    sl[i] = new TKeyObj() { Key = key, Obj = obj };
                    RemoveCache(i);
                }
            }

            public void Add(string key, object obj) => sl.Add(new TKeyObj() { Key = key, Obj = obj });

            public int Count { get => sl.Count; }

            public Type ValueType => new TKeyObj() { }.ValueType;

            #endregion
        }
    }
}
