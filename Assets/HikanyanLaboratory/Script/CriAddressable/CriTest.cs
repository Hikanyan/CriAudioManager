using System;
using CriWare;
using CriWare.Assets;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace HikanyanLaboratory
{
    public class CriTest : MonoBehaviour
    {
        [SerializeField] private AssetReferenceT<CriAtomAcbAsset> acbAssetReference; // LoadするAcbアセット
        [SerializeField] private CriAtomCueReference _cueReference; // サウンドの指定
        private CriAtomExPlayer _player; // 再生するプレイヤー

        private async void Start()
        {
            _player = new CriAtomExPlayer();
            var cueAsset = await LoadAcbAssetAsync();

            if (cueAsset != null)
            {
                RegisterAndPlayCue(cueAsset);
            }
        }

        private async UniTask<CriAtomAcbAsset> LoadAcbAssetAsync()
        {
            var handle = acbAssetReference.LoadAssetAsync<CriAtomAcbAsset>();
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.Log("CueAsset loaded successfully");
                return handle.Result;
            }
            else
            {
                Debug.LogError($"Failed to load Cue: {acbAssetReference.AssetGUID}, Error: {handle.OperationException}");
                Addressables.Release(handle);
                return null;
            }
        }

        private async void RegisterAndPlayCue(CriAtomAcbAsset cueAsset)
        {
            CriAtomAssetsLoader.AddCueSheet(cueAsset);

            // ロード完了を待機
            await UniTask.WaitUntil(() => CriAtomAssetsLoader.Instance.GetCueSheet(cueAsset)?.AcbAsset.Loaded == true);

            // キューIDの再生設定と再生
            _player.SetCue(cueAsset.Handle, _cueReference.CueId);
            _player.Start();
            Debug.Log("Sound playback started.");
        }

        private void OnDestroy()
        {
            DisposePlayer();
            ReleaseCueSheet();
        }

        private void DisposePlayer()
        {
            _player?.Dispose();
            _player = null;
            GC.SuppressFinalize(this);
        }

        private void ReleaseCueSheet()
        {
            if (acbAssetReference.Asset != null)
            {
                CriAtomAssetsLoader.ReleaseCueSheet((CriAtomAcbAsset)acbAssetReference.Asset, true);
            }
        }
    }
}
