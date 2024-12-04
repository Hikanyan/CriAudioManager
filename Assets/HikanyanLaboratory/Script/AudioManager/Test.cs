using UnityEngine;

namespace HikanyanLaboratory.Audio.Audio_Manager
{
    public class Test : MonoBehaviour
    {
        public void Start()
        {
            AudioManager.Instance.Initialize();
            AudioManager.Instance.SetMasterVolume(0.8f);
            AudioManager.Instance.PlayBGM("music_00");
            AudioManager.Instance.PlaySE("gun1_High");
            AudioManager.Instance.Enable3DSound(true);
        }
    }
}