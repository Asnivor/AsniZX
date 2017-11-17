using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsniZX.SubSystem.Sound
{
    public interface ISoundOutput : IDisposable
    {
        void StartSound();
        void StopSound();
        void ApplyVolumeSettings(double volume);
        int MaxSamplesDeficit { get; }
        int CalculateSamplesNeeded();
        void WriteSamples(short[] samples, int sampleCount);
    }
}
