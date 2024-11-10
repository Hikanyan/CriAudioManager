using System;
using System.Collections;
using CriWare.Assets;
using Cysharp.Threading.Tasks;
using HikanyanLaboratory;
using R3;
using UnityEngine;
using UnityEngine.TestTools;

public class CriAddressableAudioManagerTests : MonoBehaviour
{
    [SerializeField] private CueReference cueReference;
    [SerializeField, Range(0f, 1f)] private float value = 0.5f;

    private CriAddressableAudioManager _audioManager;
    private CriAddressableAudioManager.SimplePlayback _playback;
    private ReactiveProperty<float> _valueReactive;

    private async void Start()
    {
        // ReactivePropertyの初期化
        _valueReactive = new ReactiveProperty<float>(value);

        // 値の変更を監視し、更新があったら処理を実行
        _valueReactive.Subscribe(newValue =>
        {
            Debug.Log($"Value changed: {newValue}");
            UpdateValue(newValue);
        }).AddTo(this);

        _audioManager = CriAddressableAudioManager.Instance;

        // 再生を開始し、完了を待機
        _playback = await _audioManager.StartPlayback(cueReference);
        await UniTask.Delay(TimeSpan.FromSeconds(1f));
        _playback.Pause();
        _playback.Resume();
        await UniTask.Delay(TimeSpan.FromSeconds(1f));
        _playback.Stop();
    }

    private void Update()
    {
        if (_valueReactive != null)
        {
            _valueReactive.Value = value;
        }
    }

    // Inspector上で値が変更された際に呼ばれる
    private void OnValidate()
    {
        // ReactivePropertyの値を更新
        if (_valueReactive != null)
        {
            _valueReactive.Value = value;
        }
    }

    private void UpdateValue(float newValue)
    {
        if (_playback.IsPlaying())
        {
            _playback.SetVolume(newValue);
            Debug.Log($"Updated Value to: {newValue}");
        }
    }
}