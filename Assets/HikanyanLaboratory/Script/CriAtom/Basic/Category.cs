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
                    PlayCue(info.CueId);
                });
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(obj);
            }
        }

        public List<CriAtomSourceForAsset> SetCues()
        {
            List<CriAtomSourceForAsset> atomSources = new List<CriAtomSourceForAsset>();
            foreach (var cue in cueList)
            {
                GameObject gameObject = new GameObject($"{cue}Source");
                CriAtomSourceForAsset atomSource = gameObject.AddComponent<CriAtomSourceForAsset>();
                atomSource.Cue = cue;
                atomSources.Add(atomSource);
            }

            return atomSources;
        }

        void Unload()
        {
            if (currentLoaded != null)
            {
                Destroy(currentLoaded);
                currentLoaded = null;
            }
        }

        void PlayCue(int cueId)
        {
            if (currentLoaded == null)
            {
                currentLoaded = new GameObject($"{currentInfo.CueId}Source");
            }
            CriAtomSourceForAsset atomSource = currentLoaded.AddComponent<CriAtomSourceForAsset>();
            atomSource.Cue = currentInfo;
            atomSource.Play(cueId);
        }

        void SetVolume(AudioType audioType, float volume)
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