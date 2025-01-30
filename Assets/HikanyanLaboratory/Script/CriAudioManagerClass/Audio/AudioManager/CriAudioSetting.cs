using System;
using UnityEngine;
using System.Collections.Generic;
using HikanyanLaboratory.Audio;
using HikanyanLaboratory.System;

namespace HikanyanLaboratory.Audio
{
    [CreateAssetMenu(fileName = "CriAudioSetting", menuName = "HikanyanLaboratory/Audio/CriAudioSetting")]
    [Serializable]
    public class CriAudioSetting : ScriptableObject
    {
        [SerializeField] private string _streamingAssetsPathAcf;
        [SerializeField] private List<AudioCueSheet<string>> _audioCueSheet;
        [SerializeField] private SerializableDictionary<CriAudioType, List<string>, AudioCueSheetPair> _cueSheetDictionary;

        public string StreamingAssetsPathAcf => _streamingAssetsPathAcf;
        public List<AudioCueSheet<string>> AudioCueSheet => _audioCueSheet;
        public SerializableDictionary<CriAudioType, List<string>, AudioCueSheetPair> CueSheetDictionary => _cueSheetDictionary;

        public void Initialize()
        {
            _audioCueSheet ??= new List<AudioCueSheet<string>>();
            _cueSheetDictionary = new SerializableDictionary<CriAudioType, List<string>, AudioCueSheetPair>();
        }

        public void SearchCueSheet()
        {
            CriAudioLoader criAudioLoader = new CriAudioLoader();
            criAudioLoader.SetCriAudioSetting(this);
            criAudioLoader.SearchCueSheet();
        }

        public void SetStreamingAssetsPathAcf(string path)
        {
            _streamingAssetsPathAcf = path;
        }

        public void SetAudioCueSheet(List<AudioCueSheet<string>> cueSheets)
        {
            _audioCueSheet = cueSheets;
        }

        public void AddCueSheet(CriAudioType cueSheetType, List<string> cueNames)
        {
            _cueSheetDictionary.Add(cueSheetType, cueNames);
        }

        public string GetCueName(CriAudioType cueSheetType, int index)
        {
            if (_cueSheetDictionary.TryGetValue(cueSheetType, out var cueNames) && index < cueNames.Count)
            {
                return cueNames[index];
            }

            return string.Empty;
        }

        public List<string> GetCueNames(CriAudioType cueSheetType)
        {
            return _cueSheetDictionary.TryGetValue(cueSheetType, out var cueNames)
                ? cueNames : new List<string>();
        }
    }

    [Serializable]
    public class AudioCueSheetPair : Pair<CriAudioType, List<string>>
    {
        public AudioCueSheetPair(CriAudioType key, List<string> value) : base(key, value) { }
    }
}
