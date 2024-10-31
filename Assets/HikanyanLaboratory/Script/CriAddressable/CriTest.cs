using CriWare.Assets;
using UnityEngine;
using UnityEngine.Serialization;

namespace HikanyanLaboratory
{
    public class CriTest : MonoBehaviour
    {
        [SerializeField] private CriAtomCueReference _testCueReference;
        [SerializeField] private string _name = "Listener 1";
        
        void Start()
        {
            // "cueSheetAddress" は、Addressableに設定されたアセットのアドレス
            CriAddressableAudioManager.Instance.LoadCueAsync(_name, (cueReference) =>
            {
                // サウンド再生
                CriAddressableAudioManager.Instance.StartPlayback(cueReference, vol: 1.0f, pitch: 0.0f);
            });
        }

            
        
    }
}