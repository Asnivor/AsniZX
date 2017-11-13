using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AsniZX.Emulation.Hardware.Machine;

namespace AsniZX.Emulation.Interfaces
{
    /// <summary>
    /// Interface that represents a device that is attached to the emulated ZX model
    /// </summary>
    public interface IZXBoundDevice : IDevice
    {
        /// <summary>
        /// The emulated ZX instance that hosts this device
        /// </summary>
        ZXBase HostZX { get; }

        /// <summary>
        /// Signs that the device has been attached to the ZX model instance
        /// </summary>
        /// <param name="hostZX"></param>
        void OnAttached(ZXBase hostZX);
    }
}
