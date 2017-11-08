using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsniZX.ZXSpectrum.Hardware.CPU.Abstraction.Devices
{
    /// <summary>
    /// This interface represents an abstract device that is intended to specify
    /// the behavior of any device.
    /// </summary>
    public interface IDevice
    {
        /// <summary>
        /// Resets this device
        /// </summary>
        void Reset();
    }

}
