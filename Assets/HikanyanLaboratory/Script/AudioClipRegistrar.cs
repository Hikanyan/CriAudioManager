using System.Collections.Generic;
using UnityEngine;

namespace HikanyanLaboratory.Audio
{
    [CreateAssetMenu(fileName = "AudioClipRegistrar", menuName = "Audio/AudioClipRegistrar")]
    public class AudioClipRegistrar : ScriptableObject
    {
      [SerializeField] private List<AudioClip> _bgmClips;
      [SerializeField] private List<AudioClip> _seClips;
      [SerializeField] private List<AudioClip> _voiceClips;
        
        public List<AudioClip> BGMClips => _bgmClips;
        public List<AudioClip> SeClips => _seClips;
        public List<AudioClip> VoiceClips => _voiceClips;
    }
}