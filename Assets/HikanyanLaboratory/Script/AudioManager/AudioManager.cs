using System;
using System.Collections.Generic;
using CriWare;
using CriWare.Assets;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace HikanyanLaboratory.Audio.Audio_Manager
{
    public class AudioManager : PureSingleton<AudioManager>
    {
        
        private readonly Dictionary<string, CriAtomAcbAsset> _cueSheetCache = new Dictionary<string, CriAtomAcbAsset>();
        
        private CriAtomExPlayer _bgmPlayer;
        private CriAtomExPlayer _sePlayer;
        private CriAtomExPlayer _voicePlayer;

        private float _masterVolume = 1.0f;
        private float _bgmVolume = 1.0f;
        private float _seVolume = 1.0f;
        private float _voiceVolume = 1.0f;

        public void Initialize()
        {
            _bgmPlayer = new CriAtomExPlayer();
            _sePlayer = new CriAtomExPlayer();
            _voicePlayer = new CriAtomExPlayer();
        }

        #region Audio Control Methods

        public void PlayBGM(string cueName)
        {
            _bgmPlayer.SetCue(null, cueName); // キューシートは事前にロード済みと仮定
            _bgmPlayer.SetVolume(_bgmVolume * _masterVolume);
            _bgmPlayer.Start();
        }

        public void PlaySE(string cueName)
        {
            _sePlayer.SetCue(null, cueName);
            _sePlayer.SetVolume(_seVolume * _masterVolume);
            _sePlayer.Start();
        }

        public void PlayVoice(string cueName)
        {
            _voicePlayer.SetCue(null, cueName);
            _voicePlayer.SetVolume(_voiceVolume * _masterVolume);
            _voicePlayer.Start();
        }

        public void StopAll()
        {
            _bgmPlayer.Stop();
            _sePlayer.Stop();
            _voicePlayer.Stop();
        }

        public void PauseAll()
        {
            _bgmPlayer.Pause();
            _sePlayer.Pause();
            _voicePlayer.Pause();
        }

        public void ResumeAll()
        {
            _bgmPlayer.Resume(CriAtomEx.ResumeMode.AllPlayback);
            _sePlayer.Resume(CriAtomEx.ResumeMode.AllPlayback);
            _voicePlayer.Resume(CriAtomEx.ResumeMode.AllPlayback);
        }

        #endregion

        #region Volume Control Methods

        public void SetCategoryVolume(string categoryName, float volume)
        {
            CriAtom.SetCategoryVolume(categoryName, volume);
        }

        public float GetCategoryVolume(string categoryName)
        {
            return CriAtom.GetCategoryVolume(categoryName);
        }

        public void SetMasterVolume(float volume)
        {
            _masterVolume = volume;
            ApplyVolume();
        }

        public void SetBGMVolume(float volume)
        {
            _bgmVolume = volume;
            _bgmPlayer.SetVolume(_bgmVolume * _masterVolume);
        }

        public void SetSEVolume(float volume)
        {
            _seVolume = volume;
            _sePlayer.SetVolume(_seVolume * _masterVolume);
        }

        public void SetVoiceVolume(float volume)
        {
            _voiceVolume = volume;
            _voicePlayer.SetVolume(_voiceVolume * _masterVolume);
        }

        private void ApplyVolume()
        {
            SetBGMVolume(_bgmVolume);
            SetSEVolume(_seVolume);
            SetVoiceVolume(_voiceVolume);
        }

        #endregion

        #region Advanced Features

        public void LoadCueSheet(string acbPath, string awbPath = null)
        {
            CriAtomExAcb acb = CriAtomExAcb.LoadAcbFile(null, acbPath, awbPath);
            if (acb == null)
            {
                throw new Exception($"Failed to load CueSheet from {acbPath}");
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

        public void Set3DListener(CriAtomEx3dListener listener)
        {
            _bgmPlayer.Set3dListener(listener);
            _sePlayer.Set3dListener(listener);
            _voicePlayer.Set3dListener(listener);
        }

        public void Set3DSource(CriAtomEx3dSource source)
        {
            _bgmPlayer.Set3dSource(source);
            _sePlayer.Set3dSource(source);
            _voicePlayer.Set3dSource(source);
        }

        public void Enable3DSound(bool enable)
        {
            var panType = enable ? CriAtomEx.PanType.Pos3d : CriAtomEx.PanType.Pan3d;
            _bgmPlayer.SetPanType(panType);
            _sePlayer.SetPanType(panType);
            _voicePlayer.SetPanType(panType);
        }

        #endregion

        public override void Dispose()
        {
            _bgmPlayer.Dispose();
            _sePlayer.Dispose();
            _voicePlayer.Dispose();
        }
    }
}