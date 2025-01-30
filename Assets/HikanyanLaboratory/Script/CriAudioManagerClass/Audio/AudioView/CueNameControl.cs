using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HikanyanLaboratory.Audio
{
    public class CueNameControl : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _cueNameText;
        [SerializeField] private TMP_InputField _cueNameInputField;

        public void Initialize(string label)
        {
            _cueNameText.text = label;
        }

        public string GetCueName()
        {
            return _cueNameInputField.text;
        }
    }
}