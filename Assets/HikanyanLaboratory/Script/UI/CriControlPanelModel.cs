﻿using System.Collections.Generic;
using HikanyanLaboratory.Script;
using UnityEngine;

namespace HikanyanLaboratory.Audio
{
    public class CriControlPanelModel
    {
        private Dictionary<SoundType, float> _currentVolumes;
        public Dictionary<SoundType, float> GetCurrentVolumes() => _currentVolumes;

        /// <summary>
        /// 音量変更処理（スライダー）
        /// </summary>
        public void ChangeVolumeBySlider(SoundType soundType, float value)
        {
            float normalizedValue = Mathf.Clamp01(value / 100f);
            // Debug.Log($"SoundType {soundType}: スライダーで音量変更 - {normalizedValue}");
            // AudioManagerに音量を反映
            ApplyVolumeChange(soundType, normalizedValue);
        }

        /// <summary>
        /// 音量変更処理（入力フィールド）
        /// </summary>
        public void ChangeVolumeByInput(SoundType soundType, string value)
        {
            if (float.TryParse(value, out float result))
            {
                float normalizedValue = Mathf.Clamp01(result / 100f);
                // Debug.Log($"SoundType {soundType}: 入力で音量変更 - {normalizedValue}");
                // AudioManagerに音量を反映
                ApplyVolumeChange(soundType, normalizedValue);
            }
            else
            {
                Debug.LogWarning($"SoundType {soundType}: 入力が無効です - {value}");
            }
        }

        /// <summary>
        /// 音量変更を反映する
        /// </summary>
        private void ApplyVolumeChange(SoundType soundType, float normalizedValue)
        {
            switch (soundType)
            {
                case SoundType.MASTER:
                    
                    break;
                case SoundType.BGM:
                    
                    break;
                case SoundType.SE:
                    
                    break;
                case SoundType.VOICE:
                    
                    break;
                default:
                    Debug.LogWarning($"未対応のSoundType: {soundType}");
                    break;
            }
        }
    }
}