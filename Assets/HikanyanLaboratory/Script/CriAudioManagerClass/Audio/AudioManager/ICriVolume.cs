using R3;

namespace HikanyanLaboratory.Audio
{
    public interface ICriVolume
    {
        ReactiveProperty<float> Volume { get; }
        void SetVolume(float volume);
    }
}