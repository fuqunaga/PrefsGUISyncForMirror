using UnityEngine;
using UnityEngine.Networking;

#pragma warning disable 0618  

namespace PrefsGUI.Sync.UNET
{
    public partial class PrefsGUISyncUNET : NetworkBehaviour
    {
        public interface ISyncListKeyObj
        {
            (string key, object obj) Get(int i);
            void Set(int i, string key, object obj);
            void Add(string key, object obj);
            int Count { get; }
        }


        #region KeyObj

        public struct KeyBool { public string key; public bool value; }
        public struct KeyInt { public string key; public int value; }
        public struct KeyUInt { public string key; public uint value; }
        public struct KeyFloat { public string key; public float value; }
        public struct KeyString { public string key; public string value; }
        public struct KeyColor { public string key; public Color value; }
        public struct KeyVector2 { public string key; public Vector2 value; }
        public struct KeyVector3 { public string key; public Vector3 value; }
        public struct KeyVector4 { public string key; public Vector4 value; }
        public struct KeyVector2Int { public string key; public Vector2Int_ value; } // UNET don't support Vector2Int
        public struct KeyVector3Int { public string key; public Vector3Int_ value; }
        public struct KeyRect { public string key; public Rect value; }
        public struct KeyBounds { public string key; public Bounds_ value; }
        public struct KeyBoundsInt { public string key; public BoundsInt_ value; }

        #endregion


        #region SyncListKeyObj

        public class SyncListKeyBool : SyncListStruct<KeyBool>, ISyncListKeyObj
        {
            public (string key, object obj) Get(int i) { var v = this[i]; return (v.key, v.value); }
            public void Set(int i, string key, object obj) => this[i] = new KeyBool() { key = key, value = (bool)obj };
            public void Add(string key, object obj) => Add(new KeyBool() { key = key, value = (bool)obj });
        }
        
        public class SyncListKeyInt : SyncListStruct<KeyInt>, ISyncListKeyObj
        {
            public (string key, object obj) Get(int i) { var v = this[i]; return (v.key, v.value); }
            public void Set(int i, string key, object obj) => this[i] = new KeyInt() { key = key, value = (int)obj };
            public void Add(string key, object obj) => Add(new KeyInt() { key = key, value = (int)obj });
        }

        public class SyncListKeyUInt : SyncListStruct<KeyUInt>, ISyncListKeyObj
        {
            public (string key, object obj) Get(int i) { var v = this[i]; return (v.key, v.value); }
            public void Set(int i, string key, object obj) => this[i] = new KeyUInt() { key = key, value = (uint)obj };
            public void Add(string key, object obj) => Add(new KeyUInt() { key = key, value = (uint)obj });
        }

        public class SyncListKeyFloat : SyncListStruct<KeyFloat>, ISyncListKeyObj
        {
            public (string key, object obj) Get(int i) { var v = this[i]; return (v.key, v.value); }
            public void Set(int i, string key, object obj) => this[i] = new KeyFloat() { key = key, value = (float)obj };
            public void Add(string key, object obj) => Add(new KeyFloat() { key = key, value = (float)obj });
        }

        public class SyncListKeyString : SyncListStruct<KeyString>, ISyncListKeyObj
        {
            public (string key, object obj) Get(int i) { var v = this[i]; return (v.key, v.value); }
            public void Set(int i, string key, object obj) => this[i] = new KeyString() { key = key, value = (string)obj };
            public void Add(string key, object obj) => Add(new KeyString() { key = key, value = (string)obj });
        }

        public class SyncListKeyColor : SyncListStruct<KeyColor>, ISyncListKeyObj
        {
            public (string key, object obj) Get(int i) { var v = this[i]; return (v.key, v.value); }
            public void Set(int i, string key, object obj) => this[i] = new KeyColor() { key = key, value = (Color)obj };
            public void Add(string key, object obj) => Add(new KeyColor() { key = key, value = (Color)obj });
        }

        public class SyncListKeyVector2 : SyncListStruct<KeyVector2>, ISyncListKeyObj
        {
            public (string key, object obj) Get(int i) { var v = this[i]; return (v.key, v.value); }
            public void Set(int i, string key, object obj) => this[i] = new KeyVector2() { key = key, value = (Vector2)obj };
            public void Add(string key, object obj) => Add(new KeyVector2() { key = key, value = (Vector2)obj });
        }

        public class SyncListKeyVector3 : SyncListStruct<KeyVector3>, ISyncListKeyObj
        {
            public (string key, object obj) Get(int i) { var v = this[i]; return (v.key, v.value); }
            public void Set(int i, string key, object obj) => this[i] = new KeyVector3() { key = key, value = (Vector3)obj };
            public void Add(string key, object obj) => Add(new KeyVector3() { key = key, value = (Vector3)obj });
        }

        public class SyncListKeyVector4 : SyncListStruct<KeyVector4>, ISyncListKeyObj
        {
            public (string key, object obj) Get(int i) { var v = this[i]; return (v.key, v.value); }
            public void Set(int i, string key, object obj) => this[i] = new KeyVector4() { key = key, value = (Vector4)obj };
            public void Add(string key, object obj) => Add(new KeyVector4() { key = key, value = (Vector4)obj });
        }

        public class SyncListKeyVector2Int : SyncListStruct<KeyVector2Int>, ISyncListKeyObj
        {
            public (string key, object obj) Get(int i) { var v = this[i]; return (v.key, (Vector2Int)v.value); }
            public void Set(int i, string key, object obj) => this[i] = new KeyVector2Int() { key = key, value = (Vector2Int)obj };
            public void Add(string key, object obj) => Add(new KeyVector2Int() { key = key, value = (Vector2Int)obj });
        }

        public class SyncListKeyVector3Int : SyncListStruct<KeyVector3Int>, ISyncListKeyObj
        {
            public (string key, object obj) Get(int i) { var v = this[i]; return (v.key, (Vector3Int)v.value); }
            public void Set(int i, string key, object obj) => this[i] = new KeyVector3Int() { key = key, value = (Vector3Int)obj };
            public void Add(string key, object obj) => Add(new KeyVector3Int() { key = key, value = (Vector3Int)obj });
        }

        public class SyncListKeyRect : SyncListStruct<KeyRect>, ISyncListKeyObj
        {
            public (string key, object obj) Get(int i) { var v = this[i]; return (v.key, v.value); }
            public void Set(int i, string key, object obj) => this[i] = new KeyRect() { key = key, value = (Rect)obj };
            public void Add(string key, object obj) => Add(new KeyRect() { key = key, value = (Rect)obj });
        }

        public class SyncListKeyBounds : SyncListStruct<KeyBounds>, ISyncListKeyObj
        {
            public (string key, object obj) Get(int i) { var v = this[i]; return (v.key, (Bounds)v.value); }
            public void Set(int i, string key, object obj) => this[i] = new KeyBounds() { key = key, value = (Bounds)obj };
            public void Add(string key, object obj) => Add(new KeyBounds() { key = key, value = (Bounds)obj });
        }

        public class SyncListKeyBoundsInt : SyncListStruct<KeyBoundsInt>, ISyncListKeyObj
        {
            public (string key, object obj) Get(int i) { var v = this[i]; return (v.key, (BoundsInt)v.value); }
            public void Set(int i, string key, object obj) => this[i] = new KeyBoundsInt() { key = key, value = (BoundsInt)obj };
            public void Add(string key, object obj) => Add(new KeyBoundsInt() { key = key, value = (BoundsInt)obj });
        }

        #endregion


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
    }
}
