using UnityEngine;

namespace PrefsGUI.Sync.Example
{
    /// <summary>
    /// prefsがsync前にGet()された後でsyncされた場合、sync前の値がアプリケーションで使われている
    /// これは意図した挙動でない可能性が高いのでメッセージを出す
    /// </summary>
    public class PreSyncAccessTest : MonoBehaviour
    {
        public PrefsFloat prefs = new(nameof(PreSyncAccessTest));

        void Start()
        {
            prefs.Set(Random.value);
            
            // クライアント側ではここでクライアントがアクティブになる前にアクセスしてるので
            // PrefsGUISyncForMirror.Update()でメッセージが出るはず
            prefs.Get();
        }
    }
}