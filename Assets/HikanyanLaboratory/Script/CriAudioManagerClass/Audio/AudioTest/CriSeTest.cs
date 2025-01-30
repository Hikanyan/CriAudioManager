using System.Collections.Generic;
using CriWare;
using UnityEngine;

namespace SoulRunProject
{
    public class CriSeTest : MonoBehaviour
    {
        private List<CriAtomExPlayer> _sePlayers;
        [SerializeField] private string _cueSheetSe = "CueSheet_SE"; // キューシートの名前
        private CriAtomExAcb _acb;
        private CriAtomEx3dSource _3dSource;
        private CriAtomListener _listener;
        private CriAtomSource _atomSource;

        private void Start()
        {
            // ACBファイルをロード
            _acb = CriAtom.GetCueSheet(_cueSheetSe).acb;
            if (_acb == null)
            {
                Debug.LogError("ACBファイルがロードされていません。");
                return;
            }

            // 3Dソースとリスナーを初期化
            _3dSource = new CriAtomEx3dSource();
            _3dSource.SetPosition(0, 0, 0);
            _3dSource.Update();
            _listener = FindObjectOfType<CriAtomListener>();
            if (_listener == null)
            {
                Debug.LogWarning($"{nameof(CriAtomListener)} が見つかりません。");
            }

            // CriAtomExPlayerのリストを初期化
            _sePlayers = new List<CriAtomExPlayer>();

            // キューシートからすべてのキュー名を取得して再生
            PlayAllCues();
        }

        private void PlayAllCues()
        {
            // キューシートからすべてのキュー情報を取得
            List<string> cueNames = GetAllCueNames(_cueSheetSe);

            // 各キューを個別のプレイヤーで再生
            foreach (var cueName in cueNames)
            {
                CriAtomExPlayer player = new CriAtomExPlayer();
                player.Set3dSource(_3dSource);
                if (_listener != null)
                {
                    player.Set3dListener(_listener.nativeListener);
                }
                PlaySE(player, cueName);
                _sePlayers.Add(player);
            }

            Debug.Log("すべてのキューが再生されました。");
        }

        private void PlaySE(CriAtomExPlayer player, string cueName)
        {
            if (_acb == null)
            {
                Debug.LogError("ACBがロードされていません。");
                return;
            }

            player.SetCue(_acb, cueName);
            player.Start();
        }


        private List<string> GetAllCueNames(string cueSheetName)
        {
            List<string> cueNames = new List<string>();
            var acb = CriAtom.GetCueSheet(cueSheetName).acb;
            if (acb != null)
            {
                CriAtomEx.CueInfo[] cueInfos = acb.GetCueInfoList();
                foreach (var cueInfo in cueInfos)
                {
                    cueNames.Add(cueInfo.name);
                }
            }

            return cueNames;
        }

        private void OnDestroy()
        {
            // すべてのプレイヤーからリスナーを解除
            foreach (var player in _sePlayers)
            {
                if (_listener != null)
                {
                    player.Set3dListener(null);
                }

                player.Dispose();
            }

            // 3Dソースを破棄
            if (_3dSource != null)
            {
                _3dSource.Dispose();
            }
        }
    }
}