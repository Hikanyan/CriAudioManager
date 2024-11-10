using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using R3;

namespace HikanyanLaboratory
{
    // 曲のリストのオブジェクトは軽量化のため使い回しをする


    public class MusicPlayer : MonoBehaviour
    {
        [SerializeField] private Button _PlayButton;
        [SerializeField] private Button _nextButton;
        [SerializeField] private Button _BackButton;
        [SerializeField] private Slider _VolumeSlider;
        [SerializeField] private Slider _progressSlider;
        [SerializeField] private Dropdown _MusicDropdown;

        [SerializeField] private Text _musicNameText;
        [SerializeField] private List<CueReference> _audioList = new List<CueReference>();

        private CriAddressableAudioManager _criAddressableAudioManager;
        private CriAddressableAudioManager.SimplePlayback _currentPlayback;

        public ReactiveProperty<int> CurrentIndex { get; private set; } = new ReactiveProperty<int>(0);
        public ReactiveProperty<float> Volume { get; private set; } = new ReactiveProperty<float>(1f); // ボリューム


        void Start()
        {
            _criAddressableAudioManager = CriAddressableAudioManager.Instance;
            CurrentIndex.Subscribe(PlayMisic);
            _nextButton.onClick.AddListener(Next);
            _BackButton.onClick.AddListener(Back);
            _MusicDropdown.ClearOptions();
            _MusicDropdown.options.Add(new Dropdown.OptionData { text = "None" });
        }

        /// <summary>
        /// インデックスで再生を変更
        /// </summary>
        /// <param name="playIndex"></param>
        public async void PlayMisic(int playIndex)
        {
            _currentPlayback = await _criAddressableAudioManager.StartPlayback(_audioList[playIndex]);
            // Debug.Log(_audioList[playIndex].ToString());
        }

        /// <summary>
        /// 次のCueを再生する
        /// </summary>
        public void Next()
        {
            CurrentIndex.Value++;
        }

        /// <summary>
        /// 前のCueを再生する
        /// </summary>
        public void Back()
        {
            CurrentIndex.Value--;
        }

        public void SetVolume(float volume)
        {
            _currentPlayback.SetVolume(volume);
        }

        private void AudioSet()
        {
            // foreach (Dropdown.OptionData audio in _audioList)
            // {
            //     _MusicDropdown.options.Add(audio);
            // }
        }
    }
}