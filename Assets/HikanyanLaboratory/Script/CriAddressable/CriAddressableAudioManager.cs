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
    public class SimplifiedCueReference
    {
        public string cueSheetAddress;  // Addressableのキューシートアドレス
        public int cueId;               // 再生するCue ID
    }

    public class CriAddressableAudioManager : IDisposable
    {
        private CriAtomExPlayer _player;
        private bool _disposedValue;

        // キューシートのキャッシュ
        private Dictionary<string, CriAtomAcbAsset> _cueSheetCache = new Dictionary<string, CriAtomAcbAsset>();

        public static CriAddressableAudioManager Instance { get; } = new CriAddressableAudioManager();

        private CriAddressableAudioManager()
        {
            _player = new CriAtomExPlayer();
        }

        /// <summary>
        /// 指定されたキューを再生（キューシートが未登録ならAddressableAssetsからロードして登録）
        /// </summary>
        public async UniTask<SimplePlayback> StartPlayback(SimplifiedCueReference cueReference, float volume = 1.0f, float pitch = 0)
        {
            var cueSheet = await LoadAndRegisterCueSheet(cueReference.cueSheetAddress);

            // キューシートがロードされていれば再生
            if (cueSheet != null)
            {
                _player.SetCue(cueSheet.Handle, cueReference.cueId);
                _player.SetVolume(volume);
                _player.SetPitch(pitch);
                return new SimplePlayback(_player, _player.Start());
            }
            else
            {
                Debug.LogError($"Failed to start playback: CueSheet '{cueReference.cueSheetAddress}' not found.");
                return default;
            }
        }

        /// <summary>
        /// キューシートをロードし、キャッシュに保存
        /// </summary>
        private async UniTask<CriAtomAcbAsset> LoadAndRegisterCueSheet(string cueSheetAddress)
        {
            // キャッシュにあるか確認
            if (_cueSheetCache.TryGetValue(cueSheetAddress, out var cachedCueSheet))
            {
                Debug.Log($"Using cached CueSheet: '{cueSheetAddress}'");
                return cachedCueSheet;
            }

            // キャッシュにない場合、Addressableからロード
            AsyncOperationHandle<CriAtomCueReference> handle = Addressables.LoadAssetAsync<CriAtomCueReference>(cueSheetAddress);
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                var cueReference = handle.Result;
                CriAtomAssetsLoader.AddCueSheet(cueReference.AcbAsset);  // キューシートを登録

                // キャッシュに保存
                _cueSheetCache[cueSheetAddress] = cueReference.AcbAsset;
                Debug.Log($"Loaded and registered CueSheet: '{cueSheetAddress}'");
                return cueReference.AcbAsset;
            }
            else
            {
                Debug.LogError($"Failed to load CueSheet: {cueSheetAddress}, Error: {handle.OperationException}");
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

            public void SetVolumeAndPitch(float volume, float pitch)
            {
                _player.SetVolume(volume);
                _player.SetPitch(pitch);
                _player.Update(_playback);
            }

            public void Stop() => _playback.Stop();
            public bool IsPlaying() => _playback.GetStatus() == CriAtomExPlayback.Status.Playing;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _player?.Dispose();
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
