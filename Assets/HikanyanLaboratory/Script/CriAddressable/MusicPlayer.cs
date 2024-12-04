using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using R3;

namespace HikanyanLaboratory
{
    public class MusicPlayer : MonoBehaviour
    {
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _nextButton;
        [SerializeField] private Button _backButton;
        [SerializeField] private Slider _volumeSlider;
        [SerializeField] private Slider _progressSlider;
        [SerializeField] private Dropdown _musicDropdown;

        [SerializeField] private Text _musicNameText;
        [SerializeField] private List<CueReference> _audioList = new List<CueReference>();

        private CriAddressableAudioManager _criAddressableAudioManager;
        private CriAddressableAudioManager.SimplePlayback? _currentPlayback;
        [SerializeField] SerializableReactiveProperty<int> _musicNameReactiveProperty = new ();
        public ReactiveProperty<int> CurrentIndex { get; private set; } = new ReactiveProperty<int>(0);
        public ReactiveProperty<float> Volume { get; private set; } = new ReactiveProperty<float>(1f);

        void Start()
        {
            _criAddressableAudioManager = CriAddressableAudioManager.Instance;

            // 曲の再生とボリューム設定の反映
            CurrentIndex.Subscribe(PlayMusic);
            _volumeSlider.onValueChanged.AddListener(SetVolume);

            // ボタンイベントの設定
            _playButton.onClick.AddListener(StartMusic);
            _nextButton.onClick.AddListener(Next);
            _backButton.onClick.AddListener(Back);

            // Dropdownの初期設定
            InitializeDropdown();

            // Dropdownの選択変更時イベント
            _musicDropdown.onValueChanged.AddListener(DropdownValueChanged);
        }

        private void InitializeDropdown()
        {
            _musicDropdown.ClearOptions();
            List<string> options = new List<string> { "None" }; // 最初に"None"オプションを追加
            options.AddRange(_audioList.ConvertAll(audio => audio.ToString()));
            _musicDropdown.AddOptions(options);
        }

        /// <summary>
        /// インデックスで再生を変更
        /// </summary>
        /// <param name="playIndex"></param>
        public async void PlayMusic(int playIndex)
        {
            if (playIndex < 0 || playIndex >= _audioList.Count) return;

            _currentPlayback?.Stop();

            _currentPlayback = await _criAddressableAudioManager.StartPlayback(_audioList[playIndex]);
            _musicNameText.text = _audioList[playIndex].ToString();
        }

        public void StartMusic()
        {
          //  PlayMusic(CurrentIndex);
        }

        /// <summary>
        /// 次のCueを再生する
        /// </summary>
        public void Next()
        {
            if (CurrentIndex.Value < _audioList.Count - 1)
            {
                CurrentIndex.Value++;
            }
        }

        /// <summary>
        /// 前のCueを再生する
        /// </summary>
        public void Back()
        {
            if (CurrentIndex.Value > 0)
            {
                CurrentIndex.Value--;
            }
        }

        /// <summary>
        /// ボリュームを設定
        /// </summary>
        /// <param name="volume"></param>
        public void SetVolume(float volume)
        {
            _currentPlayback?.SetVolume(volume);
            Volume.Value = volume;
        }

        private void DropdownValueChanged(int value)
        {
            // "None"オプションの分インデックスを調整
            CurrentIndex.Value = value - 1;
        }
    }
}
