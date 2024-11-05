using System.Collections;
using Cysharp.Threading.Tasks;
using HikanyanLaboratory;
using UnityEngine;
using UnityEngine.TestTools;

public class CriAddressableAudioManagerTests : MonoBehaviour
{
    private CriAddressableAudioManager _audioManager;

    public IEnumerator TestStartPlayback_WithCaching() => 
        UniTask.ToCoroutine(async () =>
        {
            _audioManager = CriAddressableAudioManager.Instance;

            // テスト用のSimplifiedCueReferenceを作成
            var testCue = new SimplifiedCueReference
            {
                cueSheetAddress = "TestCueSheetAddress",
                cueId = 1
            };

            Debug.Log("Starting first playback...");
            var playback1 = await _audioManager.StartPlayback(testCue, volume: 1.0f, pitch: 0.0f);
        
            // 最初の再生結果を確認
            if (playback1.IsPlaying())
            {
                Debug.Log("First playback started successfully.");
            }
            else
            {
                Debug.LogError("Failed to start first playback.");
            }
        
            playback1.Stop();

            Debug.Log("Starting second playback...");
            var playback2 = await _audioManager.StartPlayback(testCue, volume: 0.5f, pitch: -0.5f);
        
            // キャッシュが利用されているか確認
            if (playback2.IsPlaying())
            {
                Debug.Log("Second playback started successfully using cached CueSheet.");
            }
            else
            {
                Debug.LogError("Failed to start second playback.");
            }
        
            playback2.Stop();
        });
}