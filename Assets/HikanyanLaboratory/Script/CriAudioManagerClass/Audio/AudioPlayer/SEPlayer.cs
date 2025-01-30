using CriWare;
using R3;

namespace HikanyanLaboratory.Audio
{
    public class SEPlayer : CriAudioPlayerService
    {
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public SEPlayer(string cueSheetName, CriAtomListener listener)
            : base(cueSheetName, listener)
        {
            Observable.EveryUpdate()
                .Subscribe(_ => CheckPlayerStatus())
                .AddTo(_disposables);
        }

        ~SEPlayer()
        {
            _disposables.Dispose();
        }
    }
}