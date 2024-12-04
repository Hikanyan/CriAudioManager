using System;
using System.Collections.Generic;
using CriWare;
using CriWare.Assets;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace HikanyanLaboratory
{
    [Serializable]
    public class CueReference
    {
        public AssetReferenceT<CriAtomAcbAsset> cueSheetAddress; // Addressableのキューシートアドレス
        public CriAtomCueReference cueId; // 再生するCue ID
    }

    public class CriAddressableAudioManager : IDisposable
    {
        private CriAtomExPlayer _player;
        private bool _disposedValue;

        // キューシートのキャッシュ
        // ハッシュセット
        private readonly Dictionary<string, CriAtomAcbAsset> _cueSheetCache = new Dictionary<string, CriAtomAcbAsset>();
        private readonly HashSet<CriAtomAcbAsset> _cueIdCache = new ();
        public static CriAddressableAudioManager Instance { get; } = new CriAddressableAudioManager();

        private CriAddressableAudioManager()
        {
            _player = new CriAtomExPlayer();
        }

        /// <summary>
        /// 指定されたキューを再生（キューシートが未登録ならAddressableAssetsからロードして登録）
        /// </summary>
        public async UniTask<SimplePlayback> StartPlayback(CueReference cueReference, float volume = 1.0f,
            float pitch = 0)
        {
            var cueSheet = await LoadAndRegisterCueSheet(cueReference.cueSheetAddress);

            // キューシートがロードされていれば再生
            if (cueSheet != null)
            {
                _player.SetCue(cueSheet.Handle, cueReference.cueId.CueId);
                _player.SetVolume(volume);
                _player.SetPitch(pitch);
                return new SimplePlayback(_player, _player.Start());
            }
            else
            {
                Debug.LogError(
                    $"Failed to start playback: CueSheet '{cueReference.cueSheetAddress.AssetGUID}' or CueID '{cueReference.cueId.CueId}' not found.");
                return default;
            }
        }

        /// <summary>
        /// キューシートをロードし、キャッシュに保存
        /// </summary>
        private async UniTask<CriAtomAcbAsset> LoadAndRegisterCueSheet(AssetReferenceT<CriAtomAcbAsset> cueSheetAddress)
        {
            string assetKey = cueSheetAddress.AssetGUID;

            // キャッシュにあるか確認
            if (_cueSheetCache.TryGetValue(assetKey, out var cachedCueSheet))
            {
                Debug.Log($"Using cached CueSheet: '{assetKey}'");
                return cachedCueSheet;
            }

            // キャッシュにない場合、Addressableからロード
            AsyncOperationHandle<CriAtomAcbAsset> handle = cueSheetAddress.LoadAssetAsync();
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                var cueSheet = handle.Result;
                CriAtomAssetsLoader.AddCueSheet(cueSheet); // キューシートを登録
                await UniTask.WaitUntil(() =>
                    CriAtomAssetsLoader.Instance.GetCueSheet(cueSheet)?.AcbAsset.Loaded == true);

                // キャッシュに保存
                _cueSheetCache[assetKey] = cueSheet;
                Debug.Log($"Loaded and registered CueSheet: '{assetKey}'");
                return cueSheet;
            }
            else
            {
                Debug.LogError($"Failed to load CueSheet: {assetKey}, Error: {handle.OperationException}");
                return null;
            }
        }

        public CriAtomExPlayer Player => _player;

        /// <summary>
        /// 再生用のPlayback構造体
        /// </summary>
        public struct SimplePlayback
        {
            private readonly CriAtomExPlayer _player;
            private CriAtomExPlayback _playback;

            internal SimplePlayback(CriAtomExPlayer player, CriAtomExPlayback playback)
            {
                _player = player;
                _playback = playback;
            }

            public void Pause() => _playback.Pause();
            public void Resume() => _playback.Resume(CriAtomEx.ResumeMode.PausedPlayback);
            public bool IsPaused() => _playback.IsPaused();
            public void Stop() => _playback.Stop();
            public bool IsPlaying() => _playback.GetStatus() == CriAtomExPlayback.Status.Playing;
            public long GetTime() => _playback.GetTime();
            public long GetTimeSyncedWithAudio() => _playback.GetTimeSyncedWithAudio();

            public void SetVolume(float volume)
            {
                _player.SetVolume(volume);
                _player.Update(_playback);
            }

            public void SetPitch(float pitch)
            {
                _player.SetPitch(pitch);
                _player.Update(_playback);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _player?.Dispose();

                    // キャッシュされたキューシートのリリース
                    foreach (var cueSheet in _cueSheetCache.Values)
                    {
                        CriAtomAssetsLoader.ReleaseCueSheet(cueSheet, true);
                    }

                    _cueSheetCache.Clear();
                }

                _player = null;
                _disposedValue = true;
            }
        }

        ~CriAddressableAudioManager()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}