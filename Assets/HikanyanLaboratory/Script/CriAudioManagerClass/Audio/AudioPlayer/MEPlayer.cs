using System;
using System.Collections.Generic;
using CriWare;
using R3;
using UnityEngine;

namespace HikanyanLaboratory.Audio
{
    public class MEPlayer : CriAudioPlayerService
    {
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public MEPlayer(string cueSheetName, CriAtomListener listener)
            : base(cueSheetName, listener)
        {
            Observable.EveryUpdate()
                .Subscribe(_ => CheckPlayerStatus())
                .AddTo(_disposables);
        }

        protected override void PrePlayCheck(string cueName)
        {
            // BGM 再生時には既存の BGM を止める
            StopAllME();
        }

        private void StopAllME()
        {
            var idsToStop = new List<Guid>(_playbacks.Keys);
            foreach (var id in idsToStop)
            {
                Stop(id);
            }
        }

        ~MEPlayer()
        {
            _disposables.Dispose();
        }
    }
}