using AsniZX.ZXSpectrum.Hardware.CPU.Abstraction.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsniZX.ZXSpectrum.Abstraction
{
    /// <summary>
    /// Interface that designates the class it bound to CPU T-States
    /// </summary>
    public interface ICpuBound : IDevice
    {
        /// <summary>
        /// Get the current T-State of the device
        /// </summary>
        long TStates { get; }
    }
}
