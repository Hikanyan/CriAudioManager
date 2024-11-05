using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class Controller : MonoBehaviour
{
    // AssetReferenceという型で直観的に参照できる
    [SerializeField] private AssetReference assetReference;

    private AsyncOperationHandle<Texture2D> textureHandle;

    private async void Start()
    {
        // SpriteRendererコンポーネントの取得
        var spriteRenderer = GetComponent<SpriteRenderer>();

        // AssetReferenceからTexture2Dとしてアセットを非同期で読み込み
        textureHandle = assetReference.LoadAssetAsync<Texture2D>();
        var texture = await textureHandle.Task;

        // Texture2DからSpriteを生成して割り当て
        if (texture != null)
        {
            var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            spriteRenderer.sprite = sprite;
        }
        else
        {
            Debug.LogWarning("Failed to load the texture.");
        }
    }

    private void OnDestroy()
    {
        // リソースを解放
        if (textureHandle.IsValid())
        {
            Addressables.Release(textureHandle);
        }
    }
}