using System;
using CriWare;
using CriWare.Assets;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace HikanyanLaboratory
{
    /// <summary>
    ///　TODO : AddresableとCriを使用した汎用性のある便利なManagerを作成する
    /// 
    /// </summary>
    public class CriAddressableAudioManager : IDisposable
    {
        private CriAtomExPlayer _player;

        public CriAtomExPlayer Player
        {
            get => _player;
        }

        public static CriAddressableAudioManager Instance { get; } = new CriAddressableAudioManager();
        private bool _disposedValue;

        /// <summary>
        /// CueSheetやCueのロード用のメソッド
        /// </summary>
        public async void LoadCueAsync(string cueSheetAddress, Action<CriAtomCueReference> onLoaded)
        {
            // AddressableからCriAtomCueReferenceをロード
            AsyncOperationHandle<CriAtomCueReference> handle =
                Addressables.LoadAssetAsync<CriAtomCueReference>(cueSheetAddress);
            await handle.Task; // ロード完了まで待機

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                CriAtomCueReference cueReference = handle.Result;
                onLoaded?.Invoke(cueReference);
            }
            else
            {
                Debug.LogError($"Failed to load Cue: {cueSheetAddress}, Error: {handle.OperationException}");
            }
        }

        /// <summary>
        /// 再生用のPlayback
        /// </summary>
        public struct SimplePlayback
        {
            private readonly CriAtomExPlayer _player;
            CriAtomExPlayback _playback;

            internal SimplePlayback(CriAtomExPlayer player, CriAtomExPlayback pb)
            {
                _player = player;
                _playback = pb;
            }

            public void Pause()
            {
                _playback.Pause();
            }

            public void Resume()
            {
                _playback.Resume(CriAtomEx.ResumeMode.PausedPlayback);
            }

            public bool IsPaused()
            {
                return _playback.IsPaused();
            }

            public void SetVolumeAndPitch(float vol, float pitch)
            {
                _player.SetVolume(vol);
                _player.SetPitch(pitch);
                _player.Update(_playback);
            }

            public void Stop()
            {
                _playback.Stop();
            }

            public bool IsPlaying()
            {
                return _playback.GetStatus() == CriAtomExPlayback.Status.Playing;
            }
        }

        public SimplePlayback StartPlayback(CriAtomCueReference cue, float vol = 1.0f, float pitch = 0)
        {
            Player.SetCue(cue.AcbAsset.Handle, cue.CueId);
            Player.SetVolume(vol);
            Player.SetPitch(pitch);
            SimplePlayback pb = new SimplePlayback(Player, Player.Start());
            return pb;
        }

        private CriAddressableAudioManager()
        {
            _player = new CriAtomExPlayer();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                _player?.Dispose();
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