using CriWare;
using R3;

namespace HikanyanLaboratory.Audio
{
    public class VoicePlayer : CriAudioPlayerService
    {
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public VoicePlayer(string cueSheetName, CriAtomListener listener)
            : base(cueSheetName, listener)
        {
            Observable.EveryUpdate()
                .Subscribe(_ => CheckPlayerStatus())
                .AddTo(_disposables);
        }


        ~VoicePlayer()
        {
            _disposables.Dispose();
        }
    }
}