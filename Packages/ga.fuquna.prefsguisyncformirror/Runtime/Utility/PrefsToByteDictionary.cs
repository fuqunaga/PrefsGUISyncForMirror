using System.Collections.Generic;

namespace PrefsGUI.Sync
{
    public class PrefsToByteDictionary<T>
    {
        private readonly Dictionary<string, T> _cacheTable = new();
        private readonly IDictionary<string, byte[]> _targetDictionary;

        public PrefsToByteDictionary(IDictionary<string, byte[]> targetDictionary)
        {
            _targetDictionary = targetDictionary;
        }
        
        public void RemoveCache(string key) => _cacheTable.Remove(key);
        
        public void WriteFrom(IPrefsInnerAccessor<T> prefsInnerAccessor)
        {
            var key = prefsInnerAccessor.Key;
            var value = prefsInnerAccessor.Get();
            
            if (_cacheTable.TryGetValue(key, out var cachedValue) && prefsInnerAccessor.Equals(value, cachedValue))
            {
                return;
            }

            _cacheTable[key] = value;


            _targetDictionary.TryGetValue(key, out var bytes);
            BytesConverter.ValueToBytes(value, ref bytes);
            
            _targetDictionary[key] = bytes;
        }
    }
}