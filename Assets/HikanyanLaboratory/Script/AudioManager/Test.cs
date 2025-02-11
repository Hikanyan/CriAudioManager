using System;
using System.Threading.Tasks;
using CriWare;
using CriWare.Assets;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;
using R3;

namespace HikanyanLaboratory.Audio
{
    public class Test : MonoBehaviour
    {
        [SerializeField] private CueReference _cueReference;
        private CriAtomExPlayback _currentPlayback;

        public void Start()
        {
            AudioManager.Instance.SetVolume(AudioManager.CriAudioType.Master, 0.8f);
            _currentPlayback = AudioManager.Instance.Play(AudioManager.CriAudioType.BGM, _cueReference);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                AudioManager.Instance.Pause(_currentPlayback);
            }
        }
    }
}