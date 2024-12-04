using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;

public class InputFieldView : MonoBehaviour
{
    [SerializeField] private TMP_InputField _inputFieldView;

    public static string SoundNeko
    {
        get { return "Neko"; }
        set { }
    }
    
    public void Awake()
    {
        _inputFieldView.gameObject.SetActive(false);
    }
    
    private void OnEnable()
    {
        if (_inputFieldView != null)
        {
            Debug.Log($"Activating input field: {_inputFieldView.name}");
            ActivateInputFieldWithDelay().Forget();
        }
        else
        {
            Debug.LogWarning("InputFieldView is not assigned in the inspector.");
        }
    }

    private async UniTaskVoid ActivateInputFieldWithDelay()
    {
        await UniTask.Delay( TimeSpan.FromSeconds(3f), cancellationToken: CancellationToken.None);
        
        _inputFieldView.gameObject.SetActive(true); // 念のためアクティブ化
        _inputFieldView.Select(); // 入力フィールドを選択状態にする
        _inputFieldView.ActivateInputField(); // 入力待機を開始

        Debug.Log("Input field is now active and ready for input.");
    }

}