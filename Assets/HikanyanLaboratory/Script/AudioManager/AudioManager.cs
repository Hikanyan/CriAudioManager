using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CriWare;
using CriWare.Assets;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace HikanyanLaboratory.Audio
{
    public class AudioManager : PureSingleton<AudioManager>
    {
        private readonly Dictionary<string, CriAtomAcbAsset> _cueSheetCache = new Dictionary<string, CriAtomAcbAsset>();

        private readonly Dictionary<CriAudioType, CriAtomExPlayer> _audioPlayers =
            new Dictionary<CriAudioType, CriAtomExPlayer>
            {
                { CriAudioType.BGM, new CriAtomExPlayer() },
                { CriAudioType.SE, new CriAtomExPlayer() },
                { CriAudioType.VOICE, new CriAtomExPlayer() }
            };

        private float _masterVolume = 1.0f;
        private float _bgmVolume = 1.0f;
        private float _seVolume = 1.0f;
        private float _voiceVolume = 1.0f;

        public enum CriAudioType
        {
            Master,
            BGM,
            SE,
            VOICE,
        }

        #region Audio Control Methods

        public CriAtomExPlayback Play(CriAudioType audioType, CueReference cueReference)
        {
            if (!_audioPlayers.TryGetValue(audioType, out var player))
            {
                Debug.LogError($"Audio type {audioType} not found.");
            }

            // _cueSheetCache に再生するCue SheetがなければLoadする
            
            var cueSheet = await LoadAndRegisterCueSheet(cueReference.cueSheetAddress);

            // キューシートがロードされていれば再生
            if (cueSheet != null)
            {
                player.SetCue(cueSheet.Handle, cueReference.cueId.CueId);
                player.SetVolume(GetVolume(audioType));

                return player.Start();
            }
            else
            {
                Debug.LogError(
                    $"Failed to start playback: CueSheet '{cueReference.cueSheetAddress.AssetGUID}' or CueID '{cueReference.cueId.CueId}' not found.");
            }
        }

        private async UniTask<CriAtomExAcb> GetAcbAsync(string cueSheetName)
        {
            CriAtomExAcb acb = null;

            while (acb == null && !string.IsNullOrEmpty(cueSheetName))
            {
                acb = CriAtom.GetAcb(cueSheetName);
                if (acb == null)
                {
                    Debug.Log($"Waiting for ACB '{cueSheetName}' to be available...");
                    await UniTask.Yield();
                }
            }

            if (acb != null)
            {
                Debug.Log($"ACB '{cueSheetName}' successfully retrieved.");
            }

            return acb;
        }

        public void Stop(CriAtomExPlayback playbackInfo)
        {
            playbackInfo.Stop();
        }


        public void Pause(CriAtomExPlayback playbackInfo)
        {
            playbackInfo.Pause();
        }

        public void Resume(CriAtomExPlayback playbackInfo, CriAtomEx.ResumeMode resumeMode)
        {
            playbackInfo.Resume(resumeMode);
        }

        public void StopAll()
        {
            foreach (var player in _audioPlayers.Values)
            {
                player.Stop();
            }
        }

        public void PauseAll()
        {
            foreach (var player in _audioPlayers.Values)
            {
                player.Pause();
            }
        }

        public void ResumeAll()
        {
            foreach (var player in _audioPlayers.Values)
            {
                player.Resume(CriAtomEx.ResumeMode.AllPlayback);
            }
        }

        private float GetVolume(CriAudioType audioType)
        {
            return audioType switch
            {
                CriAudioType.Master => _masterVolume,
                CriAudioType.BGM => _bgmVolume * _masterVolume,
                CriAudioType.SE => _seVolume * _masterVolume,
                CriAudioType.VOICE => _voiceVolume * _masterVolume,
                _ => throw new ArgumentOutOfRangeException(nameof(audioType), audioType, null)
            };
        }

        #endregion

        #region Volume Control Methods

        public void SetVolume(CriAudioType audioType, float volume)
        {
            switch (audioType)
            {
                case CriAudioType.Master:
                    _masterVolume = Mathf.Clamp01(volume);
                    break;
                case CriAudioType.BGM:
                    _bgmVolume = Mathf.Clamp01(volume);
                    break;
                case CriAudioType.SE:
                    _seVolume = Mathf.Clamp01(volume);
                    break;
                case CriAudioType.VOICE:
                    _voiceVolume = Mathf.Clamp01(volume);
                    break;
            }

            ApplyVolume();
        }

        private void ApplyVolume()
        {
            foreach (var type in _audioPlayers.Keys)
            {
                if (_audioPlayers.TryGetValue(type, out var player))
                {
                    player.SetVolume(GetVolume(type));
                }
            }
        }

        #endregion

        #region CueSheet Management

        private async UniTask<CriAtomAcbAsset> LoadAndRegisterCueSheet(AssetReferenceT<CriAtomAcbAsset> cueSheetAddress)
        {
            string assetKey = cueSheetAddress.AssetGUID;

            if (_cueSheetCache.TryGetValue(assetKey, out var cachedCueSheet))
            {
                Debug.Log($"Using cached CueSheet: '{assetKey}'");
                return cachedCueSheet;
            }

            AsyncOperationHandle<CriAtomAcbAsset> handle = cueSheetAddress.LoadAssetAsync();
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                var cueSheet = handle.Result;
                CriAtomAssetsLoader.AddCueSheet(cueSheet);
                await UniTask.WaitUntil(() =>
                    CriAtomAssetsLoader.Instance.GetCueSheet(cueSheet)?.AcbAsset.Loaded == true);

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

        #endregion

        #region 3D Audio

        public void Set3DListener(CriAtomEx3dListener listener)
        {
            foreach (var player in _audioPlayers.Values)
            {
                player.Set3dListener(listener);
            }
        }

        public void Set3DSource(CriAtomEx3dSource source)
        {
            foreach (var player in _audioPlayers.Values)
            {
                player.Set3dSource(source);
            }
        }

        public void Enable3DSound(bool enable)
        {
            var panType = enable ? CriAtomEx.PanType.Pos3d : CriAtomEx.PanType.Pan3d;
            foreach (var player in _audioPlayers.Values)
            {
                player.SetPanType(panType);
            }
        }

        #endregion

        public override void Dispose()
        {
            foreach (var player in _audioPlayers.Values)
            {
                player.Dispose();
            }

            _audioPlayers.Clear();
        }
    }
}