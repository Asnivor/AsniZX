using AsniZX.ZXSpectrum.Hardware.CPU.Abstraction.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsniZX.ZXSpectrum.Abstraction
{
    public interface IInterrupt : IDevice, IFrameBound, ISpectrumAttachable
    {
        /// <summary>
        /// The ULA T-State to raise the interrupt at
        /// </summary>
        int InterruptTState { get; }

        /// <summary>
        /// Whether or not an interrupt has been raised in this frame
        /// </summary>
        bool InterruptRaised { get; }

        /// <summary>
        /// Whether or not an interrupt has been revoked in this frame
        /// </summary>
        bool InterruptRevoked { get; }

        /// <summary>
        /// Checks whether an interrupt is due in the current frame
        /// and raises one if neccessary
        /// </summary>
        /// <param name="currentTState"></param>
        void CheckForInterrupt(int currentTState);
    }
}
