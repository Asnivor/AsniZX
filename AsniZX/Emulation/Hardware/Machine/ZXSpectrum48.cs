using AsniZX.Emulation.Hardware.CPU.SpectnetideZ80;
using AsniZX.Emulation.Hardware.Display;
using AsniZX.Emulation.Hardware.IO;
using AsniZX.Emulation.Hardware.Memory;
using AsniZX.Emulation.Interfaces;
using AsniZX.SubSystem.Display;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsniZX.Emulation.Hardware.Machine
{
    public class ZXSpectrum48 : ZXBase
    {

        #region Construction

        public ZXSpectrum48(EmulationMachine machine) 
        {
            // pass in the machine class
            _Machine = machine;

            // the DisplayHandler class
            DHandler = _Machine.displayHandler;

            // init screen configuration
            ScreenConfiguration = new ScreenConfig();
            ScreenConfiguration.RefreshRate = 50;
            ScreenConfiguration.FlashToggleFrames = 25;
            ScreenConfiguration.VerticalSyncLines = 8;
            ScreenConfiguration.NonVisibleBorderTopLines = 8;// 8; // --- In a real screen this value is 0
            ScreenConfiguration.BorderTopLines = 48; // --- In a real screen this value is 55
            ScreenConfiguration.BorderBottomLines = 48; // --- In a real screen this value is 56
            ScreenConfiguration.NonVisibleBorderBottomLines = 8; // --- In a real screen this value is 0
            ScreenConfiguration.DisplayLines = 192;
            ScreenConfiguration.ScreenLines = 
                ScreenConfiguration.BorderTopLines 
                + ScreenConfiguration.DisplayLines 
                + ScreenConfiguration.BorderBottomLines;
            ScreenConfiguration.FirstDisplayLine = 
                ScreenConfiguration.VerticalSyncLines 
                + ScreenConfiguration.NonVisibleBorderTopLines 
                + ScreenConfiguration.BorderTopLines;
            ScreenConfiguration.LastDisplayLine = 
                ScreenConfiguration.FirstDisplayLine 
                + ScreenConfiguration.DisplayLines - 1;
            ScreenConfiguration.BorderLeftPixels = 48;// 48;
            ScreenConfiguration.BorderRightPixels = 48;// 48;
            ScreenConfiguration.DisplayWidth = 256;
            ScreenConfiguration.ScreenWidth = 
                ScreenConfiguration.BorderLeftPixels 
                + ScreenConfiguration.DisplayWidth 
                + ScreenConfiguration.BorderRightPixels;
            ScreenConfiguration.HorizontalBlankingTime = 40;
            ScreenConfiguration.BorderLeftTime = 24;
            ScreenConfiguration.DisplayLineTime = 128;
            ScreenConfiguration.BorderRightTime = 24;
            ScreenConfiguration.NonVisibleBorderRightTime = 8;
            ScreenConfiguration.PixelDataPrefetchTime = 2;
            ScreenConfiguration.AttributeDataPrefetchTime = 1;
            ScreenConfiguration.FirstPixelTStateInLine 
                = ScreenConfiguration.HorizontalBlankingTime 
                + ScreenConfiguration.BorderLeftTime;
            ScreenConfiguration.ScreenLineTime 
                = ScreenConfiguration.FirstPixelTStateInLine 
                + ScreenConfiguration.DisplayLineTime 
                + ScreenConfiguration.BorderRightTime 
                + ScreenConfiguration.NonVisibleBorderRightTime;
            ScreenConfiguration.UlaFrameTStateCount 
                = (ScreenConfiguration.FirstDisplayLine 
                + ScreenConfiguration.DisplayLines 
                + ScreenConfiguration.BorderBottomLines 
                + ScreenConfiguration.NonVisibleBorderTopLines) 
                * ScreenConfiguration.ScreenLineTime;
            ScreenConfiguration.FirstDisplayPixelTState 
                = ScreenConfiguration.FirstDisplayLine 
                * ScreenConfiguration.ScreenLineTime
                + ScreenConfiguration.HorizontalBlankingTime 
                + ScreenConfiguration.BorderLeftTime;
            ScreenConfiguration.FirstScreenPixelTState 
                = (ScreenConfiguration.VerticalSyncLines 
                + ScreenConfiguration.NonVisibleBorderTopLines) 
                * ScreenConfiguration.ScreenLineTime
                + ScreenConfiguration.HorizontalBlankingTime;


            // setup ZXSpectrum48 defaults
            CpuClockFrequency = 3500000;
            TStatesPerFrame = 69888;
            InterruptTState = 32;

            // clock
            Clock = ClockProvider;
            Clock?.OnAttached(this);

            // init CPU
            MemoryDevice = new Memory();
            PortDevice = new Port();
            Cpu = new Z80Cpu(MemoryDevice, PortDevice);

            // init other devices
            InterruptDevice = new Interrupt(InterruptTState);
            BorderDevice = new Border();
            ScreenDevice = new Screen();

            // attach devices
            _spectrumDevices.Add(MemoryDevice);
            _spectrumDevices.Add(PortDevice);
            _spectrumDevices.Add(InterruptDevice);
            _spectrumDevices.Add(BorderDevice);
            _spectrumDevices.Add(ScreenDevice);

            // setup devices
            MemoryDevice.OnAttached(this);
            PortDevice.OnAttached(this);
            InterruptDevice.OnAttached(this);
            BorderDevice.OnAttached(this);
            ScreenDevice.OnAttached(this);

            // frame calculations
            ResetUlaTState();
            _tStatesPerFrame = ScreenDevice.ScreenConfiguration.UlaFrameTStateCount;
            PhysicalFrameClockCount = Clock.GetFrequency() / (double)CpuClockFrequency * _tStatesPerFrame;
            FrameCount = 0;
            OverFlowTStates = 0;
            _frameCompleted = true;
            RunsInMaskableInterrupt = false;

            _frameBoundDevices = _spectrumDevices.OfType<IFrameBoundDevice>().ToList();
            _cpuBoundDevices = _spectrumDevices.OfType<ICpuOperationBoundDevice>().ToList();

                  

            // load ROM
            Rom.LoadRom(MachineType.ZXSpectrum48, MemoryDevice);
        }

        #endregion

        #region Per-Model Device Configuration

        /// <summary>
        /// Handles memory read/writes
        /// </summary>
        public class Memory : MemoryDeviceBase
        {
            public Memory()
            {
                // set total memory size (ram + rom)
                MemorySize = 0x10000;       // 48k + 16k
            }

            /// <summary>
            /// Reads the memory at the specified address
            /// </summary>
            /// <param name="addr">Memory address</param>
            /// <returns>Byte read from the memory</returns>
            public override byte OnReadMemory(ushort addr)
            {
                var data = _memory[addr];
                if ((addr & 0xC000) == 0x4000)
                {
                    // Address is in the RAM range (above 16k)
                    // Apply contention if neccessary
                    _cpu.Delay(_screenDevice.GetContentionValue(HostZX.CurrentFrameTState));
                }
                return data;
            }

            /// <summary>
            /// Sets the memory value at the specified address
            /// </summary>
            /// <param name="addr">Memory address</param>
            /// <param name="value">Memory value to write</param>
            /// <returns>Byte read from the memory</returns>
            public override void OnWriteMemory(ushort addr, byte value)
            {
                // Check whether memory is ROM or RAM
                switch (addr & 0xC000)
                {
                    case 0x0000:
                        // Do nothing - we cannot write to ROM
                        return;
                    case 0x4000:
                        // Address is RAM - apply contention if neccessary
                        _cpu.Delay(_screenDevice.GetContentionValue(HostZX.CurrentFrameTState));
                        break;
                }
                _memory[addr] = value;
            }

            /// <summary>
            /// The ULA reads the memory at the specified address
            /// </summary>
            /// <param name="addr">Memory address</param>
            /// <returns>Byte read from the memory</returns>
            /// <remarks>
            /// We need this device to emulate the contention for the screen memory
            /// between the CPU and the ULA.
            /// </remarks>
            public override byte OnULAReadMemory(ushort addr)
            {
                var value = _memory[(addr & 0x3FFF) + 0x4000];
                return value;
            }


        }

        /// <summary>
        /// Handles port read/writes
        /// </summary>
        public class Port : PortDeviceBase
        {
            /// <summary>
            /// Reads the port with the specified address
            /// </summary>
            /// <param name="addr"></param>
            /// <returns></returns>
            public override byte OnReadPort(ushort addr)
            {
                // Every even I/O address will address the ULA
                // Ignore everything else
                if ((addr & 0x0001) != 0)
                    return 0xFF;

                /*The lowest three bits specify the border colour; a zero in bit 3 activates the MIC output, 
                 * whilst a one in bit 4 activates the EAR output and the internal speaker. 
                 * However, the EAR and MIC sockets are connected only by resistors, so activating one activates the other; 
                 * the EAR is generally used for output as it produces a louder sound. The upper two bits are unused.
                Bit   7   6   5   4   3   2   1   0
                    +-------------------------------+
                    |   |   |   | E | M |   Border  |
                    +-------------------------------+
                */

                /*
                var portBits = _keyboardDevice.GetLineStatus((byte)(addr >> 8));
                var earBit = _tapeDevice.GetEarBit(_cpu.Tacts);
                if (!earBit)
                {
                    portBits = (byte)(portBits & 0b1011_1111);
                }
                return portBits;
                */

                return new byte();
            }

            /// <summary>
            /// Sends a byte of data to the specified port address
            /// </summary>
            /// <param name="addr"></param>
            /// <param name="data"></param>
            public override void OnWritePort(ushort addr, byte data)
            {
                // Only even addresses address the ULA
                if ((addr & 0x0001) == 0)
                {
                    // border
                    _borderDevice.BorderColour = data & 0x07;

                    // sound

                    // tape
                }
            }
        }

        /// <summary>
        /// Handles raising vblank interrupts
        /// </summary>
        public class Interrupt : InterruptBase
        {

            public Interrupt(int interruptTState) 
                : base(interruptTState)
            {
                InterruptTState = interruptTState;
                Longest_Operation_TStates = 23;
            }
        }

        /// <summary>
        /// Representing the machine border
        /// </summary>
        public class Border : BorderBase
        {

        }

        /// <summary>
        /// Represents the ULA screen
        /// </summary>
        public class Screen : ScreenBase
        {
            public override void OnAttached(ZXBase hostZX)
            {
                base.OnAttached(hostZX);

                HostZX = hostZX;
                displayHandler = hostZX.DHandler;
                _borderDevice = hostZX.BorderDevice;
                _fetchScreenMemory = hostZX.MemoryDevice.OnULAReadMemory;
                ScreenConfiguration = hostZX.ScreenConfiguration;
                InitializeUlaTStateTable();
                _flashPhase = false;
                FrameCount = 0;

                // --- Calculate color conversion table
                _flashOffColors = new int[0x200];
                _flashOnColors = new int[0x200];

                for (var attr = 0; attr < 0x100; attr++)
                {
                    var ink = (attr & 0x07) | ((attr & 0x40) >> 3);
                    var paper = ((attr & 0x38) >> 3) | ((attr & 0x40) >> 3);
                    _flashOffColors[attr] = paper;
                    _flashOffColors[0x100 + attr] = ink;
                    _flashOnColors[attr] = (attr & 0x80) != 0 ? ink : paper;
                    _flashOnColors[0x100 + attr] = (attr & 0x80) != 0 ? paper : ink;
                }

                _screenWidth = hostZX.ScreenDevice.ScreenConfiguration.ScreenWidth;
                _pixelBuffer = new byte[_screenWidth * hostZX.ScreenDevice.ScreenConfiguration.ScreenLines];
            }
        }

        #endregion

    }
}
