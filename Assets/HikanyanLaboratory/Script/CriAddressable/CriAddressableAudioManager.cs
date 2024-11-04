using System;
using CriWare;
using CriWare.Assets;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace HikanyanLaboratory
{
    /// <summary>
    /// AddressableとCriを使用した便利なAudioManager
    /// </summary>
    public class CriAddressableAudioManager : IDisposable
    {
        private CriAtomExPlayer _player;
        private bool _disposedValue;

        public static CriAddressableAudioManager Instance { get; } = new CriAddressableAudioManager();

        private CriAddressableAudioManager()
        {
            _player = new CriAtomExPlayer();
        }

        /// <summary>
        /// キューシートが登録されていなければ、Addressableからロードして登録
        /// </summary>
        public async void LoadAndRegisterCueSheet(string cueSheetAddress, Action<CriAtomCueReference> onLoaded)
        {
            // キューシートが既に登録済みならロード不要
            if (CriAtomAssetsLoader.Instance.GetCueSheet(cueSheetAddress) != null)
            {
                Debug.Log($"CueSheet '{cueSheetAddress}' is already loaded.");
                return;
            }

            // キューシートを非同期でロード
            AsyncOperationHandle<CriAtomCueReference> handle = Addressables.LoadAssetAsync<CriAtomCueReference>(cueSheetAddress);
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                CriAtomCueReference cueReference = handle.Result;
                CriAtomAssetsLoader.AddCueSheet(cueReference.AcbAsset);
                onLoaded?.Invoke(cueReference);
                Debug.Log($"CueSheet '{cueSheetAddress}' loaded and registered successfully.");
            }
            else
            {
                Debug.LogError($"Failed to load CueSheet: {cueSheetAddress}, Error: {handle.OperationException}");
            }
        }

        /// <summary>
        /// 指定されたキューを再生
        /// </summary>
        public Playback StartPlayback(CriAtomCueReference cue, float vol = 1.0f, float pitch = 0)
        {
            Player.SetCue(cue.AcbAsset.Handle, cue.CueId);
            Player.SetVolume(vol);
            Player.SetPitch(pitch);
            Playback playback = new Playback(Player, Player.Start());
            return playback;
        }

        public CriAtomExPlayer Player => _player;

        public struct Playback
        {
            private readonly CriAtomExPlayer _player;
            private CriAtomExPlayback _playback;

            internal Playback(CriAtomExPlayer player, CriAtomExPlayback playback)
            {
                _player = player;
                _playback = playback;
            }

            public void Pause() => _playback.Pause();
            public void Resume() => _playback.Resume(CriAtomEx.ResumeMode.PausedPlayback);
            public bool IsPaused() => _playback.IsPaused();

            public void SetVolumeAndPitch(float vol, float pitch)
            {
                _player.SetVolume(vol);
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
