using System.Threading.Tasks;
using CriWare;
using CriWare.Assets;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HikanyanLaboratory.Audio.Audio_Manager
{
    public class Test : MonoBehaviour
    {
        [SerializeField] CueReference cueReference;
        private Task<CriAtomExPlayback> _currentPlayback;

        public void Start()
        {
            AudioManager.Instance.SetMasterVolume(0.8f);
            _currentPlayback = AudioManager.Instance.Play(AudioManager.CriAudioType.BGM, cueReference);
        }
    }
}