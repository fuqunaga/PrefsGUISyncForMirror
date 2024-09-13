using System.Collections.Generic;
using System.Linq;
using Mirror;

namespace PrefsGUI.Sync.Test
{
    /// <summary>
    /// 大量のPrefsがある場合、クライアント接続時に巨大なデータを１回で送信しようとしてMirrorのエラーになる
    /// </summary>
    public class BigDataTest : NetworkBehaviour
    {
        public int count;
        
        public List<PrefsFloat> prefsFloatList;

        public void Start()
        {
            CreatePrefs();
        }


        private void CreatePrefs()
        {
            prefsFloatList = Enumerable.Range(0, count)
                .Select(i => new PrefsFloat(nameof(PrefsFloat) + i))
                .ToList();
        }
    }
}
