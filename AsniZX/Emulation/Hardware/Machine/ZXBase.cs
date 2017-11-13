using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AsniZX.Emulation.Interfaces;
using AsniZX.SubSystem.Clock;
using System.Threading;
using AsniZX.Emulation.Hardware.IO;
using AsniZX.Emulation.Hardware.Display;
using AsniZX.SubSystem.Display;

namespace AsniZX.Emulation.Hardware.Machine
{
    /// <summary>
    /// The base abstract class that all emulated models inherit from
    /// </summary>
    public abstract class ZXBase : IFrameBoundDevice
    {
        protected  int _tStatesPerFrame;
        protected bool _frameCompleted;

        protected  List<IDevice> _spectrumDevices = new List<IDevice>();
        protected  List<IFrameBoundDevice> _frameBoundDevices = new List<IFrameBoundDevice>();
        protected  List<ICpuOperationBoundDevice> _cpuBoundDevices = new List<ICpuOperationBoundDevice>();

        /// <summary>
        /// Instance of the parent machine class - passed in via contructor
        /// </summary>
        public EmulationMachine _Machine { get; set; }

        /// <summary>
        /// The machine CPU
        /// </summary>
        public IZ80Cpu Cpu { get; set; }

        /// <summary>
        /// The CPU clock frequency in Hz
        /// </summary>
        public int CpuClockFrequency { get; set; }

        /// <summary>
        /// The total number of T-States in one frame
        /// </summary>
        public int TStatesPerFrame { get; set; }

        /// <summary>
        /// The current TState that we are at within the frame
        /// </summary>
        public int CurrentFrameTState
        {
            get
            {
                return (int)(Cpu.Tacts - LastFrameCPUTick);
            }
        }

        /// <summary>
        /// The length of the physical frame in clock counts
        /// </summary>
        public double PhysicalFrameClockCount { get; set; }

        /// <summary>
        /// The T-State within the frame where the interrupt signal is generated
        /// </summary>
        public int InterruptTState { get; set; }

        /// <summary>
        /// The ROM file to be loaded (in a bytearray)
        /// </summary>
        public byte[] RomBytes { get; set; }

        /// <summary>
        /// Memory class used by this spectrum model
        /// </summary>
        public IMemoryDevice MemoryDevice { get; set; }

        /// <summary>
        /// Port class used by this spectrum model
        /// </summary>
        public IPortDevice PortDevice { get; set; }

        /// <summary>
        /// ULA class responsible for raising vblank interrupts
        /// </summary>
        public InterruptBase InterruptDevice { get; set; }

        /// <summary>
        /// The ULA border used in the machine
        /// </summary>
        public BorderBase BorderDevice { get; set; }

        /// <summary>
        /// Represents the parameters the ULA chip uses to render the Spectrum
        /// </summary>
        public ScreenConfig ScreenConfiguration { get; set; }

        /// <summary>
        /// The virtual device that controls the screen
        /// </summary>
        public ScreenBase ScreenDevice { get; set; }

        /// <summary>
        /// Does the actual rendering to the form handle
        /// </summary>
        public DisplayHandler DHandler { get; set; }

        /// <summary>
        /// CPU tick at which the last frame rendering started
        /// </summary>
        public long LastFrameCPUTick;

        /// <summary>
        /// The T-State at which the ULA rendered the last T-State
        /// </summary>
        public int LastRenderedULATState;

        

        /// <summary>
        /// The number of frames rendered
        /// </summary>
        public int FrameCount { get; set; }

        /// <summary>
        /// The current T-State in the frame based on the CPU tick count
        /// </summary>
        public virtual int CurrentTStateInFrame => (int)(Cpu.Tacts - LastFrameCPUTick);

        /// <summary>
        /// The number of T-States that have overflowed from the previous frame
        /// </summary>
        public int OverFlowTStates { get; set; }

        /// <summary>
        /// The clock provider for this emulation
        /// </summary>
        public ClockProvider ClockProvider = new ClockProvider();   // this will probably have to be moved up to the EmulationMachine class

        /// <summary>
        /// The clock used within this emulation
        /// </summary>
        public ClockProvider Clock { get; set; }

        /// <summary>
        /// This property indicates if the machine currently runs the
        /// maskable interrupt method.
        /// </summary>
        public bool RunsInMaskableInterrupt { get; set; }




        /// <summary>
        /// Main execution cycle
        /// </summary>
        public virtual bool ExecuteCycle(CancellationToken token)
        {  
            // calculate lag time at the end of the frame
            var cycleStartTime = Clock.GetCounter();
            var cycleFrameCount = 0;

            // holds the total lag that should be introduced because of pausing
            long totalLagTime = 0;

            // Main Loop - runs until cancelled
            while (!token.IsCancellationRequested)
            {
                _Machine._pauseEvent.WaitOne(Timeout.Infinite);
                if (_Machine._shutdownEvent.WaitOne(0))
                    break;

                /*
                // handle pausing
                if (_Machine.IsPaused)
                {                    
                    Thread.Sleep(200);
                    // get start of this frame
                    var thisFrameStartTime = Clock.GetCounter();
                    // get end of last frame
                    var endOfLastFrame = (cycleStartTime + cycleFrameCount * PhysicalFrameClockCount) + PhysicalFrameClockCount;
                    // increment totalLagTime by the amount we paused
                    //cycleStartTime += thisFrameStartTime - (long)endOfLastFrame;
                    totalLagTime += (long)thisFrameStartTime - (long)endOfLastFrame;
                    continue; 
                }
                */


                if (_frameCompleted)
                {
                    // frame has been completed - get last frame CPU tick
                    LastFrameCPUTick = Cpu.Tacts - OverFlowTStates;

                    // notify all devices to start a new frame
                    OnNewFrame();

                    // set the last rendered ULA T-State
                    LastRenderedULATState = OverFlowTStates;

                    _frameCompleted = false;
                }

                // Child Loop = The physical frame cycle that happens whilst ULA and CPU process everything within the frame
                while (!_frameCompleted)
                {
                    // --- Check for leaving maskable interrupt mode
                    if (RunsInMaskableInterrupt && Cpu.Registers.PC == 0x0053)
                    {
                        // --- We leave the maskable interrupt mode when the
                        // --- current instruction completes
                        RunsInMaskableInterrupt = false;
                    }



                    // check for interrupt signal generation
                    InterruptDevice.CheckForInterrupt(CurrentFrameTState);

                    // cycle the Z80 cpu once (for one instruction)
                    Cpu.ExecuteCpuCycle();

                    // run a rendering cycle based on the current CPU tact count
                    var lastTState = CurrentFrameTState;
                    ScreenDevice.RenderScreen(LastRenderedULATState + 1, lastTState);
                    LastRenderedULATState = lastTState;

                    // Current CPU operation has completed - notify CPU-bound devices
                    foreach (var dev in _cpuBoundDevices)
                    {
                        dev.OnCpuOperationCompleted();
                    }

                    // work out whether the frame has finished - true if CPU is not in the middle of an op execution
                    // and the current T-State in the frame greater than or equal to the total no. of T-states in a frame
                    _frameCompleted = !Cpu.IsInOpExecution && CurrentFrameTState >= _tStatesPerFrame;
                }

                // Physical frame has been completed
                cycleFrameCount++;
                FrameCount++;

                // Notify device about frame completion
                OnFrameCompleted();

                var freq = Clock.GetFrequency();


                var maxfps = freq / (Clock.GetCounter() - (cycleStartTime + totalLagTime + (cycleFrameCount - 1) * PhysicalFrameClockCount));// * 1000;                

                // Introduce some lag so that the frame execution time is accurate
                var nextFrameCounter = cycleStartTime + totalLagTime + (cycleFrameCount * PhysicalFrameClockCount);
                Clock.WaitUntil((long)nextFrameCounter, token);

                var actualfps = freq / ((nextFrameCounter) - (cycleStartTime + totalLagTime + (cycleFrameCount - 1) * PhysicalFrameClockCount));

                // calculate frames per second

                var frameTimeInClockCounts = cycleFrameCount * PhysicalFrameClockCount;

                _Machine.MainForm?.SetFPS((int)actualfps);
                _Machine.MainForm?.SetVirtualFPS((int)maxfps);

                // New frame starts
                OverFlowTStates = CurrentFrameTState % TStatesPerFrame; //_tStatesPerFrame;
            }

            // if we get here, the main loop has been interrupted by cancellation
            return false;
        }

        /// <summary>
        /// This flag tells if the frame has just been completed.
        /// </summary>
        public bool HasFrameCompleted => _frameCompleted;

        #region IFrameBoundDevice

        /// <summary>
        /// Allows the class to react to the start of a new frame
        /// </summary>
        public void OnNewFrame()
        {
            foreach (var dev in _frameBoundDevices)
            {
                dev.OnNewFrame();
            }
        }

        /// <summary>
        /// Allows the class to react to the completion of a frame
        /// </summary>
        public void OnFrameCompleted()
        {
            foreach (var dev in _frameBoundDevices)
            {
                dev.OverFlowTStates = OverFlowTStates;
                dev.OnFrameCompleted();
            }
        }

        public event EventHandler FrameCompleted;

        #endregion

        #region IDevice

        /// <summary>
        /// Resets ULA and CPU chips
        /// </summary>
        public virtual void Reset()
        {
            Cpu.SetResetSignal();
            ResetUlaTState();
            FrameCount = 0;
            OverFlowTStates = 0;
            _frameCompleted = true;
            foreach (var dev in _spectrumDevices)
                dev.Reset();

            Cpu.Reset();
            Cpu.ReleaseResetSignal();
        }

        #endregion

        public void ResetUlaTState()
        {
            LastRenderedULATState = -1;
        }

    }
}
