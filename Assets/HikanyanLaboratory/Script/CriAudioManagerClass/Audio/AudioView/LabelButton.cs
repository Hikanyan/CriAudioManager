using System;
using UnityEngine;
using UnityEngine.UI;

namespace HikanyanLaboratory.Audio
{
    [Serializable]
    public class LabelButton : MonoBehaviour
    {
        [SerializeField] private string _label;
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _pauseButton;
        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _stopButton;
        [SerializeField] private Toggle _loopToggle;

        private CueNameControl _cueNameControl;
        private CriAudioManager _criAudioManager;
        private CriAudioType _audioType;
        private string _cueName;
        private bool _isLoop;
        private Guid _currentPlaybackId;

        public void Initialize(string label, CriAudioType audioType, CueNameControl cueNameControl)
        {
            _criAudioManager = CriAudioManager.Instance;
            _label = label;
            _audioType = audioType;
            _cueNameControl = cueNameControl;

            _loopToggle.isOn = false;
            _isLoop = false;

            _loopToggle.onValueChanged.AddListener(Loop);
            _playButton.onClick.AddListener(Play);
            _pauseButton.onClick.AddListener(Pause);
            _resumeButton.onClick.AddListener(Resume);
            _stopButton.onClick.AddListener(Stop);
        }

        public void Play()
        {
            _cueName = _cueNameControl.GetCueName();
            if (!string.IsNullOrEmpty(_cueName))
            {
               // _currentPlaybackId = _criAudioManager.Play(_audioType, _cueName, _isLoop);
                Debug.Log($"CriAudioType: {_audioType}, CueName: {_cueName}, Loop: {_isLoop}");
            }
        }

        public void Pause()
        {
            _criAudioManager.Pause(_audioType, _currentPlaybackId);
        }

        public void Resume()
        {
            _criAudioManager.Resume(_audioType, _currentPlaybackId);
        }

        public void Stop()
        {
            _criAudioManager.Stop(_audioType, _currentPlaybackId);
        }

        public void Loop(bool isLoop)
        {
            _isLoop = isLoop;
            Debug.Log($"Loop: {_isLoop}");
        }
    }
}
