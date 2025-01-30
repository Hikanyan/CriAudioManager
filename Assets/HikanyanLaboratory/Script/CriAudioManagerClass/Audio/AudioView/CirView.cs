using R3;
using UnityEngine;
using UnityEngine.Events;

namespace HikanyanLaboratory.Audio
{
    public class CirView : MonoBehaviour
    {
        [SerializeField] private GameObject _volumeControlPrefab;
        [SerializeField] private GameObject _cueNameControlPrefab;
        [SerializeField] private Transform _volumeControlsParent;
        [SerializeField] private Transform _cueNameControlsParent;

        [SerializeField] private LabelButton _bgmButton;
        [SerializeField] private LabelButton _seButton;
        [SerializeField] private LabelButton _meButton;
        [SerializeField] private LabelButton _voiceButton;

        private CriAudioManager _criAudioManager;

        private CriVolumeController _masterCriVolumeControl;
        private CriVolumeController _bgmCriVolumeControl;
        private CriVolumeController _seCriVolumeControl;
        private CriVolumeController _meCriVolumeControl;
        private CriVolumeController _voiceCriVolumeControl;

        private CueNameControl _bgmCueNameControl;
        private CueNameControl _seCueNameControl;
        private CueNameControl _meCueNameControl;
        private CueNameControl _voiceCueNameControl;

        private void Start()
        {
            _criAudioManager = CriAudioManager.Instance;

            _masterCriVolumeControl = CreateVolumeControl("Master Volume", _criAudioManager.MasterVolume.Value,
                CriAudioType.Master, OnMasterVolumeSliderChanged, OnMasterVolumeInputChanged);
            _bgmCriVolumeControl = CreateVolumeControl("BGM Volume",
                _criAudioManager.GetPlayerVolume(CriAudioType.CueSheet_BGM), CriAudioType.CueSheet_BGM,
                OnBgmVolumeSliderChanged, OnBgmVolumeInputChanged);
            _seCriVolumeControl = CreateVolumeControl("SE Volume",
                _criAudioManager.GetPlayerVolume(CriAudioType.CueSheet_SE), CriAudioType.CueSheet_SE,
                OnSeVolumeSliderChanged, OnSeVolumeInputChanged);
            _meCriVolumeControl = CreateVolumeControl("ME Volume",
                _criAudioManager.GetPlayerVolume(CriAudioType.CueSheet_ME), CriAudioType.CueSheet_ME,
                OnMeVolumeSliderChanged, OnMeVolumeInputChanged);
            _voiceCriVolumeControl = CreateVolumeControl("Voice Volume",
                _criAudioManager.GetPlayerVolume(CriAudioType.CueSheet_Voice), CriAudioType.CueSheet_Voice,
                OnVoiceVolumeSliderChanged, OnVoiceVolumeInputChanged);

            _bgmCueNameControl = CreateCueNameControl("BGM Cue Name");
            _seCueNameControl = CreateCueNameControl("SE Cue Name");
            _meCueNameControl = CreateCueNameControl("ME Cue Name");
            _voiceCueNameControl = CreateCueNameControl("Voice Cue Name");

            _bgmButton.Initialize(_bgmCueNameControl.GetCueName(), CriAudioType.CueSheet_BGM, _bgmCueNameControl);
            _seButton.Initialize(_seCueNameControl.GetCueName(), CriAudioType.CueSheet_SE, _seCueNameControl);
            _meButton.Initialize(_meCueNameControl.GetCueName(), CriAudioType.CueSheet_ME, _meCueNameControl);
            _voiceButton.Initialize(_voiceCueNameControl.GetCueName(), CriAudioType.CueSheet_Voice,
                _voiceCueNameControl);

            BindVolumeControls();
        }

        private CueNameControl CreateCueNameControl(string label)
        {
            var cueNameControlObject = Instantiate(_cueNameControlPrefab, _cueNameControlsParent);
            var cueNameControl = cueNameControlObject.GetComponent<CueNameControl>();
            cueNameControl.Initialize(label);
            return cueNameControl;
        }

        private CriVolumeController CreateVolumeControl(string label, float initialValue, CriAudioType audioType,
            UnityAction<float> onSliderChanged, UnityAction<string> onInputChanged)
        {
            var volumeControlObject = Instantiate(_volumeControlPrefab, _volumeControlsParent);
            var volumeControl = volumeControlObject.GetComponent<CriVolumeController>();
            volumeControl.Initialize(label, initialValue, audioType, onSliderChanged, onInputChanged);
            return volumeControl;
        }

        private void OnMasterVolumeSliderChanged(float value)
        {
            _criAudioManager.MasterVolume.Value = value / 100;
            _masterCriVolumeControl.SetVolume(value / 100); // スライダーの値を直接反映
        }

        private void OnBgmVolumeSliderChanged(float value)
        {
            var player = _criAudioManager.GetPlayer(CriAudioType.CueSheet_BGM);
            if (player != null)
            {
                player.Volume.Value = value / 100;
                _bgmCriVolumeControl.SetVolume(value / 100); // スライダーの値を直接反映
            }
        }

        private void OnSeVolumeSliderChanged(float value)
        {
            var player = _criAudioManager.GetPlayer(CriAudioType.CueSheet_SE);
            if (player != null)
            {
                player.Volume.Value = value / 100;
                _seCriVolumeControl.SetVolume(value / 100); // スライダーの値を直接反映
            }
        }

        private void OnMeVolumeSliderChanged(float value)
        {
            var player = _criAudioManager.GetPlayer(CriAudioType.CueSheet_ME);
            if (player != null)
            {
                player.Volume.Value = value / 100;
                _meCriVolumeControl.SetVolume(value / 100); // スライダーの値を直接反映
            }
        }

        private void OnVoiceVolumeSliderChanged(float value)
        {
            var player = _criAudioManager.GetPlayer(CriAudioType.CueSheet_Voice);
            if (player != null)
            {
                player.Volume.Value = value / 100;
                _voiceCriVolumeControl.SetVolume(value / 100); // スライダーの値を直接反映
            }
        }

        private void OnMasterVolumeInputChanged(string value)
        {
            if (float.TryParse(value, out float floatValue))
            {
                _criAudioManager.MasterVolume.Value = floatValue / 100;
                _masterCriVolumeControl.SetVolume(floatValue / 100); // 入力フィールドの値を直接反映
            }
        }

        private void OnBgmVolumeInputChanged(string value)
        {
            if (float.TryParse(value, out float floatValue))
            {
                var player = _criAudioManager.GetPlayer(CriAudioType.CueSheet_BGM);
                if (player != null)
                {
                    player.Volume.Value = floatValue / 100;
                    _bgmCriVolumeControl.SetVolume(floatValue / 100); // 入力フィールドの値を直接反映
                }
            }
        }

        private void OnSeVolumeInputChanged(string value)
        {
            if (float.TryParse(value, out float floatValue))
            {
                var player = _criAudioManager.GetPlayer(CriAudioType.CueSheet_SE);
                if (player != null)
                {
                    player.Volume.Value = floatValue / 100;
                    _seCriVolumeControl.SetVolume(floatValue / 100); // 入力フィールドの値を直接反映
                }
            }
        }

        private void OnMeVolumeInputChanged(string value)
        {
            if (float.TryParse(value, out float floatValue))
            {
                var player = _criAudioManager.GetPlayer(CriAudioType.CueSheet_ME);
                if (player != null)
                {
                    player.Volume.Value = floatValue / 100;
                    _meCriVolumeControl.SetVolume(floatValue / 100); // 入力フィールドの値を直接反映
                }
            }
        }

        private void OnVoiceVolumeInputChanged(string value)
        {
            if (float.TryParse(value, out float floatValue))
            {
                var player = _criAudioManager.GetPlayer(CriAudioType.CueSheet_Voice);
                if (player != null)
                {
                    player.Volume.Value = floatValue / 100;
                    _voiceCriVolumeControl.SetVolume(floatValue / 100); // 入力フィールドの値を直接反映
                }
            }
        }

        private void BindVolumeControls()
        {
            _criAudioManager.GetPlayer(CriAudioType.CueSheet_BGM)?.Volume.Subscribe(volume =>
            {
                _bgmCriVolumeControl.SetVolume(volume);
            }).AddTo(this);

            _criAudioManager.GetPlayer(CriAudioType.CueSheet_SE)?.Volume.Subscribe(volume =>
            {
                _seCriVolumeControl.SetVolume(volume);
            }).AddTo(this);

            _criAudioManager.GetPlayer(CriAudioType.CueSheet_ME)?.Volume.Subscribe(volume =>
            {
                _meCriVolumeControl.SetVolume(volume);
            }).AddTo(this);

            _criAudioManager.GetPlayer(CriAudioType.CueSheet_Voice)?.Volume.Subscribe(volume =>
            {
                _voiceCriVolumeControl.SetVolume(volume);
            }).AddTo(this);

            _criAudioManager.MasterVolume.Subscribe(volume => { _masterCriVolumeControl.SetVolume(volume); })
                .AddTo(this);
        }
    }
}