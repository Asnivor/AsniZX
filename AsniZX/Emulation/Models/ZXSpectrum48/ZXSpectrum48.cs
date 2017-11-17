using AsniZX.Emulation.Hardware.CPU.SpectnetideZ80;
using AsniZX.Emulation.Hardware.Display;
using AsniZX.Emulation.Hardware.IO;
using AsniZX.Emulation.Hardware.Keyboard;
using AsniZX.Emulation.Hardware.Machine;
using AsniZX.Emulation.Hardware.Memory;
using AsniZX.Emulation.Hardware.Sound.Beeper;
using AsniZX.Emulation.Hardware.Tape;
using AsniZX.Emulation.Interfaces;
using AsniZX.SubSystem.Display;
using AsniZX.SubSystem.Sound;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsniZX.Emulation.Models.ZXSpectrum48
{
    public partial class ZXSpectrum48 : ZXBase
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

            // providers
            _beepFrameProvider = new BeeperFrameProvider(new BeeperConfig());

            // init CPU
            MemoryDevice = new Memory();
            PortDevice = new Port();
            Cpu = new Z80Cpu(MemoryDevice, PortDevice);

            // init other devices
            InterruptDevice = new Interrupt(InterruptTState);
            BorderDevice = new Border();
            ScreenDevice = new Screen();
            KeyboardDevice = new Keyboard();
            BeeperDevice = new Beeper(_beepFrameProvider);

            TapeDevice = new Tape();


            

            // attach devices
            _spectrumDevices.Add(MemoryDevice);
            _spectrumDevices.Add(PortDevice);
            _spectrumDevices.Add(InterruptDevice);
            _spectrumDevices.Add(BorderDevice);
            _spectrumDevices.Add(ScreenDevice);
            _spectrumDevices.Add(KeyboardDevice);
            _spectrumDevices.Add(BeeperDevice);
            _spectrumDevices.Add(TapeDevice);

            // setup devices
            MemoryDevice.OnAttached(this);
            PortDevice.OnAttached(this);
            InterruptDevice.OnAttached(this);
            BorderDevice.OnAttached(this);
            ScreenDevice.OnAttached(this);
            KeyboardDevice.OnAttached(this);
            BeeperDevice.OnAttached(this);
            TapeDevice.OnAttached(this);

            // setup providers
            //_beepFrameProvider.OnAttached(this);

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

        

        

        

        

        

        

        #endregion

    }
}
