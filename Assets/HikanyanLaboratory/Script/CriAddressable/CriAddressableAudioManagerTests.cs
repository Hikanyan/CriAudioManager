using System.Collections;
using CriWare.Assets;
using Cysharp.Threading.Tasks;
using HikanyanLaboratory;
using UnityEngine;
using UnityEngine.TestTools;

public class CriAddressableAudioManagerTests : MonoBehaviour
{
    [SerializeField] CueReference cueReference;
    [SerializeField, Range(0f, 1f)] private float value = 0.5f; 
    private CriAddressableAudioManager _audioManager;
    private CriAddressableAudioManager.SimplePlayback _playback;

    private async void Start()
    {
        _audioManager = CriAddressableAudioManager.Instance;

        // 再生を開始し、完了を待機
        _playback = await _audioManager.StartPlayback(cueReference);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 再生中の場合、ボリュームとピッチを変更
            if (_playback.IsPlaying())
            {
                _playback.SetVolumeAndPitch(value, -0.5f); // ボリュームを0.5、ピッチを-0.5に設定
                Debug.Log("Volume and pitch adjusted to 0.5 and -0.5.");
            }
            else
            {
                Debug.LogWarning("Playback is not active.");
            }
        }
    }
}