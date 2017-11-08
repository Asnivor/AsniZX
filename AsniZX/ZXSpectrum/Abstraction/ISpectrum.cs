using AsniZX.ZXSpectrum.Hardware.CPU;
using AsniZX.ZXSpectrum.Hardware.CPU.Abstraction.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsniZX.ZXSpectrum.Abstraction
{
    /// <summary>
    /// Interface representing a spectrum machine
    /// </summary>
    public interface ISpectrum
    {
        /// <summary>
        /// Instance of the parent machine class - passed in via contructor
        /// </summary>
        Machine _Machine { get; }

        /// <summary>
        /// The machine CPU
        /// </summary>
        Z80Cpu Cpu { get; }

        /// <summary>
        /// The CPU clock frequency in Hz
        /// </summary>
        int CpuClockFreq { get; }

        /// <summary>
        /// The total number of T-States in one frame
        /// </summary>
        int TStatesPerFrame { get; }

        /// <summary>
        /// The current TState that we are at within the frame
        /// </summary>
        int CurrentFrameTState { get; }

        /// <summary>
        /// The length of the physical frame in clock counts
        /// </summary>
        double PhysicalFrameClockCount { get; }

        /// <summary>
        /// The T-State within the frame where the interrupt signal is generated
        /// </summary>
        int InterruptTState { get; }

        /// <summary>
        /// The ROM file to be loaded (in a bytearray)
        /// </summary>
        byte[] RomBytes { get; }

        /// <summary>
        /// Memory class used by this spectrum model
        /// </summary>
        IMemoryDevice Memory { get; }

        /// <summary>
        /// Port class used by this spectrum model
        /// </summary>
        IPortDevice Port { get; }

        ////////////////////////
        /* ULA related things */
        ////////////////////////

        /// <summary>
        /// Border class used by this spectrum model
        /// </summary>
        IBorder Border { get; }

        /// <summary>
        /// ULA class responsible for rendering the screen output
        /// </summary>
        IScreen Screen { get; }

        /// <summary>
        /// ULA class responsible for raising vblank interrupts
        /// </summary>
        IInterrupt Interrupt { get; }

        /// <summary>
        /// Main execution cycle
        /// </summary>
        void ExecuteCycle();
    }
}
