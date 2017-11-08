using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsniZX.ZXSpectrum
{
    /// <summary>
    /// The different models that can be emulated
    /// </summary>
    public enum SpectrumType
    {
        ZXSpectrum16k,
        ZXSpectrum48k,
        //ZXSpectrumPlus,   // - from an emulation perspective this is identical to the 48k
        ZXSpectrum128k,
        ZXSpectrumPlus2,
        ZXSpectrumPlus2A,
        ZXSpectrumPlus3,
        ZXSpectrumPlus2B,
        ZXSpectrumPlus3B
    }
}
