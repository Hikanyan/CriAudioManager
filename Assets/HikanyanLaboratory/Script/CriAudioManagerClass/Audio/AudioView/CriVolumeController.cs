using System.Globalization;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace HikanyanLaboratory.Audio
{
    public class CriVolumeController : MonoBehaviour, ICriVolume
    {
        [SerializeField] private TextMeshProUGUI _volumeText;
        [SerializeField] private Slider _volumeSlider;
        [SerializeField] private TMP_InputField _volumeInputField;

        private string _label;
        private float _currentValue;
        private CriAudioManager _criAudioManager; // CriAudioManagerのインスタンスを保持
        private CriAudioType _audioType; // 音声タイプを保持
        public ReactiveProperty<float> Volume { get; } = new ReactiveProperty<float>(1f);

        public void Initialize(string label, float initialValue, CriAudioType audioType,
            UnityAction<float> onSliderChanged, UnityAction<string> onInputChanged)
        {
            _label = label;
            _currentValue = initialValue;
            _criAudioManager = CriAudioManager.Instance;
            _audioType = audioType;

            _volumeText.text = label;

            _volumeSlider.minValue = 0;
            _volumeSlider.maxValue = 100;
            _volumeSlider.value = initialValue * 100;
            _volumeSlider.onValueChanged.AddListener(onSliderChanged);
            _volumeSlider.onValueChanged.AddListener(OnSliderChanged);

            _volumeInputField.text = (initialValue * 100).ToString(CultureInfo.CurrentCulture);
            _volumeInputField.onEndEdit.AddListener(onInputChanged);
            _volumeInputField.onEndEdit.AddListener(OnInputChanged);

            Volume.Subscribe(SetVolume).AddTo(this);
        }

        public void SetVolume(float volume)
        {
            _currentValue = volume;
            _volumeSlider.value = volume * 100;
            _volumeInputField.text = (volume * 100).ToString(CultureInfo.CurrentCulture);
        }

        private void OnSliderChanged(float value)
        {
            _currentValue = value / 100;
            _volumeInputField.text = value.ToString(CultureInfo.CurrentCulture);
            _criAudioManager?.SetVolume(_audioType, _currentValue);
            Volume.Value = _currentValue;
        }

        private void OnInputChanged(string value)
        {
            if (float.TryParse(value, out float result))
            {
                _currentValue = result / 100;
                _volumeSlider.value = result;
                _criAudioManager?.SetVolume(_audioType, _currentValue);
                Volume.Value = _currentValue;
            }
        }
    }
}
