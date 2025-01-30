using CriWare;
using R3;

namespace HikanyanLaboratory.Audio
{
    public class OtherPlayer : CriAudioPlayerService
    {
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public OtherPlayer(string cueSheetName, CriAtomListener listener)
            : base(cueSheetName, listener)
        {
            Observable.EveryUpdate()
                .Subscribe(_ => CheckPlayerStatus())
                .AddTo(_disposables);
        }

        ~OtherPlayer()
        {
            _disposables.Dispose();
        }
    }
}