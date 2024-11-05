using System.Collections;
using CriWare.Assets;
using Cysharp.Threading.Tasks;
using HikanyanLaboratory;
using UnityEngine;
using UnityEngine.TestTools;

public class CriAddressableAudioManagerTests : MonoBehaviour
{
    
    [SerializeField] CueReference cueReference;
    private CriAddressableAudioManager _audioManager;

    async void Start()
    {
        _audioManager = CriAddressableAudioManager.Instance;
        await _audioManager.StartPlayback(cueReference);
    }
    
}