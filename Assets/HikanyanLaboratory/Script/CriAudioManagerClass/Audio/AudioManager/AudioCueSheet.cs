using System;
using UnityEngine.Serialization;

namespace HikanyanLaboratory.Audio
{
    [Serializable]
    public class AudioCueSheet<T>
    {
        public T Type;
        public string CueSheetName;
        public string AcbPath;
        public string AwbPath;
    }
}