using System.Collections.Generic;
using CriWare;
using CriWare.Assets;
using UnityEngine;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine.UI;

namespace HikanyanLaboratory.Script.CriAtom.Basic
{
    public enum AudioType
    {
        BGM,
        SE,
        VOICE,
    }

    public class Category : MonoBehaviour
    {
        [SerializeField] Transform buttonsParent = null;
        [SerializeField] GameObject buttonTemplate = null;
        [SerializeField] List<CriAtomCueReference> cueList = new List<CriAtomCueReference>();
        private CriAtomCueReference currentInfo;
        private GameObject currentLoaded = null;

        #region Variables

        private float bgmVolume = 1.0f;
        private AudioType audioTypeBgm = AudioType.BGM;

        private float seVolume = 1.0f;
        private AudioType audioTypeSe = AudioType.SE;

        #endregion

        #region Functions

        void Start()
        {
            UpdateView();
            UpdateInputKey();
        }

        async void UpdateView()
        {
            // プレハブ一覧の表示要素を一度すべて削除
            foreach (Transform t in buttonsParent)
            {
                if (t == transform) continue;
                Destroy(t.gameObject);
            }

            // プレハブ情報がなければ何も表示しない
            if (cueList == null) return;

            // プレハブ情報から各インスタンス化ボタンを生成
            foreach (var info in cueList)
            {
                var obj = Instantiate(buttonTemplate, buttonsParent);
                // 表示名を設定
                obj.GetComponentInChildren<UnityEngine.UI.Text>().text = $"{info.AcbAsset.name} : {info.CueId}";
                // クリックされた際にインスタンス化する処理を設定
                obj.GetComponentInChildren<UnityEngine.UI.Button>().onClick.AddListener(() =>
                {
                    // 他のプレハブインスタンスがあれば破棄
                    Unload();
                    currentInfo = info;
                    Debug.Log($"{currentInfo.AcbAsset.name} : {currentInfo.CueId}");
                    PlayCue(currentInfo.CueId);
                });
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(obj);
            }
        }

        public void Unload()
        {
            if (currentLoaded != null)
            {
                Destroy(currentLoaded);
            }

            currentLoaded = null;
            currentInfo = default;
        }

        // Test用
        public void UpdateInputKey()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                SetVolume(AudioType.BGM, bgmVolume += 0.1f);
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                SetVolume(AudioType.BGM, bgmVolume -= 0.1f);
            }
        }

        public void PlayCue(int cueId)
        {
            if (currentLoaded == null)
            {
                currentLoaded = new GameObject($"{currentInfo.CueId}Source");
                currentLoaded.transform.SetParent(transform);
            }

            var atomSource = currentLoaded.GetComponent<CriAtomSourceForAsset>() ??
                             currentLoaded.AddComponent<CriAtomSourceForAsset>();
            atomSource.Cue = currentInfo;

            // Handleがnullの場合はロードを試みる
            if (atomSource.Cue.AcbAsset.Handle == null)
            {
                Debug.Log("AcbAsset is not loaded. Attempting to load...");
                atomSource.Cue.AcbAsset.OnLoaded += (acbAsset) => atomSource.Play(cueId);
                atomSource.Cue.AcbAsset.LoadAsync();
                return;
            }

            atomSource.Play(cueId);
        }

        public void SetVolume(AudioType audioType, float volume)
        {
            CriWare.CriAtom.SetCategoryVolume(audioType.ToString(), volume);
        }

        void OnDisable()
        {
            CriWare.CriAtom.SetCategoryVolume(audioTypeBgm.ToString(), 1.0f);
            CriWare.CriAtom.SetCategoryVolume(audioTypeSe.ToString(), 1.0f);
        }

        #endregion
    }
}