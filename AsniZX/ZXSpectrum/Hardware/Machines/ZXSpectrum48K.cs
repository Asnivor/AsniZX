using AsniZX.ZXSpectrum.Abstraction;
using AsniZX.ZXSpectrum.Hardware.CPU;
using AsniZX.ZXSpectrum.Hardware.CPU.Abstraction.Devices;
using AsniZX.ZXSpectrum.Hardware.CPU.Abstraction.Providers;
using AsniZX.ZXSpectrum.Hardware.IO;
using AsniZX.ZXSpectrum.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsniZX.ZXSpectrum.Hardware.Machines
{
    /// <summary>
    /// 48K Spectrum Model
    /// </summary>
    public class ZXSpectrum48K : ISpectrum
    {
        #region Private Properties

        private readonly int _tStatesPerFrame;
        private bool _frameCompleted;

        #endregion

        #region ISpectrum Properties

        /// <summary>
        /// Instance of the parent machine class - passed in via contructor
        /// </summary>
        public Machine _Machine { get; }

        /// <summary>
        /// The machine CPU
        /// </summary>
        public Z80Cpu Cpu { get; }

        /// <summary>
        /// The CPU clock frequency in Hz
        /// </summary>
        public int CpuClockFreq => 3500000;

        /// <summary>
        /// The total number of T-States in one frame
        /// </summary>
        public int TStatesPerFrame => 69888;

        /// <summary>
        /// The current TState that we are at within the frame
        /// </summary>
        public int CurrentFrameTState { get; }

        /// <summary>
        /// The length of the physical frame in clock counts
        /// </summary>
        public double PhysicalFrameClockCount { get; }

        /// <summary>
        /// The T-State within the frame where the interrupt signal is generated
        /// </summary>
        public int InterruptTState => 32;

        /// <summary>
        /// The ROM file to be loaded (in a bytearray)
        /// </summary>
        public byte[] RomBytes { get; }

        /// <summary>
        /// Memory class used by this spectrum model
        /// </summary>
        public IMemoryDevice Memory { get; }

        /// <summary>
        /// Port class used by this spectrum model
        /// </summary>
        public IPortDevice Port { get; }

        /// <summary>
        /// Border class used by this spectrum model
        /// </summary>
        public IBorder Border { get; }

        /// <summary>
        /// ULA class responsible for rendering the screen output
        /// </summary>
        public IScreen Screen { get; }

        /// <summary>
        /// ULA class responsible for raising vblank interrupts
        /// </summary>
        public IInterrupt Interrupt { get; }

        #endregion

        #region Instance Properties

        /// <summary>
        /// CPU tick at which the last frame rendering started
        /// </summary>
        public long LastFrameCPUTick;

        /// <summary>
        /// The T-State at which the ULA rendered the last T-State
        /// </summary>
        public int LastRenderedULATState;

        /// <summary>
        /// The clock used within this emulation
        /// </summary>
        public IClockProvider Clock { get; }

        /// <summary>
        /// The number of frames rendered
        /// </summary>
        public int FrameCount { get; private set; }

        /// <summary>
        /// The current T-State in the frame based on the CPU tick count
        /// </summary>
        public virtual int CurrentTStateInFrame => (int)(Cpu.Tacts - LastFrameCPUTick);

        /// <summary>
        /// The number of T-States that have overflowed from the previous frame
        /// </summary>
        public int OverflowTStates { get; set; }

        /// <summary>
        /// The clock provider for this emulation
        /// </summary>
        public IClockProvider ClockProvider = new ClockProvider();

        #endregion

        #region Construction

        /// <summary>
        /// Standard constructor - takes an instance of the machine class
        /// </summary>
        /// <param name="MainForm"></param>
        public ZXSpectrum48K(Machine machine)
        {
            // pass in the machine class
            _Machine = machine;

            // init the clockprovider
            Clock = ClockProvider;

            // init memory provider
            Memory = new ZXSpectrum48KMemory();

            // init port provider
            Port = new ZXSpectrum48KPort();            

            // instantiate CPU
            Cpu = new Z80Cpu(Memory, Port);
        }

        #endregion

        #region ISpectrum Methods

        /// <summary>
        /// Main execution cycle
        /// </summary>
        public void ExecuteCycle()
        {

        }

        #endregion
    }
}
