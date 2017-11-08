using BizHawk.Emulation.Common;
using BizHawk.Z80A;
using System;
using System.Threading;

namespace AsniZX.ZXSpectrum
{
    /// <summary>
    /// The main spectrum emulation class
    /// Much of the layout of this is taken from BizHawk and is likely redundant
    /// Eventually I will work out what is needed and what is not...
    /// </summary>
    public sealed partial class Spectrum : IDebuggable, IInputPollable, IStatable, IEmulator
    {
        #region Constants

        /// <summary>
        /// Port 0xfe constants
        /// </summary>
        public int BORDER_BIT = 0x07;
        public const int EAR_BIT = 0x10;
        public const int MIC_BIT = 0x08;
        public const int TAPE_BIT = 0x40;

        #endregion

        #region Properties and Fields

        /// <summary>
        /// The model of spectrum to be emulated
        /// </summary>
        public SpecModel EmulatedModel { get; set; }       

        /// <summary>
        /// The main emulation thread
        /// </summary>
        public Thread EmulationThread { get; set; }

        /// <summary>
        /// Controls the speed of Z80 emulation
        /// 0: No modifier
        /// 1: x2
        /// 2: x2x2
        /// 3: x2x2x2
        /// 
        /// </summary>
        public int CPUMultiplier { get; set; }

        /// <summary>
        /// Frame timer
        /// </summary>
        public System.Diagnostics.Stopwatch FrameTimer { get; set; }

        public int FrameCount = 0;

        /// <summary>
        /// Whether emulation thread is paused or not
        /// </summary>
        public bool IsEmulationPaused { get; set; }

        /// <summary>
        /// CPU Instance
        /// </summary>
        public readonly Z80A _cpu = new Z80A();

        /// <summary>
        /// The spectrum ROM
        /// </summary>
        private  byte[] _rom;

        /// <summary>
        /// Spectrum RAM
        /// </summary>
        public byte[] _ram { get; set; }

        /// <summary>
        /// ROM paging array
        /// </summary>
        private byte[][] RomPage;

        /// <summary>
        /// RAM paging array
        /// </summary>
        private byte[][] RamPage;

        public ISpeccyModel Machine { get; set; }

        public ULA _ULA { get; set; }

        #endregion

        #region Construction

        public Spectrum(SpecModel model)
        {
            EmulatedModel = model;

            // start paused
            IsEmulationPaused = true;

            FrameTimer = new System.Diagnostics.Stopwatch();

            // load correct machine type
            switch (EmulatedModel)
            {
                case SpecModel._48k:
                    Machine = new _48K(this);
                    _ram = new byte[Machine.RamSize + Machine.RomSize];
                    break;

                default:
                    // no machine selected
                    return;
            }

            // Assign the func callbacks to the Z80
            _cpu.FetchMemory = Machine.ReadMemory;
            _cpu.ReadMemory = Machine.ReadMemory;
            _cpu.WriteMemory = Machine.WriteMemory;
            _cpu.ReadHardware = Machine.ReadPort;
            _cpu.WriteHardware = Machine.WritePort;
            _cpu.IRQCallback = Machine.IRQCallBack;
            _cpu.NMICallback = Machine.NMICallBack;
            _cpu.MemoryCallbacks = MemoryCallbacks;

            // Load the ULA
            _ULA = new ULA(this);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Loads the spectrum rom (16k) 
        /// </summary>
        /// <returns></returns>
        public bool LoadRom()
        {
            if (!System.IO.File.Exists(Machine.RomPath))
                return false;

            // rom is always 16k
            _rom = new byte[0x4000];
            var rData = System.IO.File.ReadAllBytes(Machine.RomPath);
            Array.Copy(rData, _rom, 0x4000);

            // load into bottom 16k of RAM
            _ram = new byte[Machine.RamSize + Machine.RomSize];
            Array.Copy(_rom, _ram, 0x4000);

            return true;
        }

        public void StartEmulation()
        {
            if (EmulationThread == null || EmulationThread.IsAlive == false)
            {
                EmulationThread = new Thread(new ThreadStart(Run));
                EmulationThread.Name = "Main Emulation Thread";
                EmulationThread.Priority = ThreadPriority.Highest;
            }
            IsEmulationPaused = false;
            HardReset();
            EmulationThread.Start();

            
        }

        public void PauseEmulation()
        {
            if (IsEmulationPaused)
                return;

            IsEmulationPaused = true;

            //EmulationThread.Join();
        }

        public void ResumeEmulation()
        {
            if (!IsEmulationPaused)
                return;

            IsEmulationPaused = false;

            if (!EmulationThread.IsAlive)
            {
                StartEmulation();
            }

            if (EmulationThread.ThreadState != ThreadState.WaitSleepJoin)
            {
                EmulationThread.Start();
            }
        }

        public void HardReset()
        {
            _cpu.Reset();
            _cpu.InterruptMode = 1;
            /*
            // fill display memory with random stuff
            Random rand = new Random();
            for (int d = Machine.DisplayStart; d < Machine.DisplayStart + 6912; ++d)
            {
                Machine.WriteMemory(Convert.ToUInt16(d), Convert.ToByte(rand.Next(255)));
            }
            _cpu.Reset();
            */


            LoadRom();
        }

        /// <summary>
        /// The main emulation loop
        /// </summary>
        public void Run()
        {
            for (;;)
            {
                // handle pause
                
                if (IsEmulationPaused)
                {
                    Thread.Sleep(100);
                    continue;
                }
                
                // start a stopwatch to time the frame
                FrameTimer.Restart();
                
                // process one frame
                ProcessFrame();                

                // stop the stopwatch
                FrameTimer.Stop();
                

                // if emulation is accurate, one frame should take 50th of a second (50Hz) - 20ms
                if (FrameTimer.ElapsedMilliseconds > (20))
                {
                    ZXForm.MainForm?.SetFPS(1000 / (int)FrameTimer.ElapsedMilliseconds);
                }
                else
                {
                    // lag the emulation
                    Thread.Sleep(Convert.ToInt32((long)(1000 / 50) - FrameTimer.ElapsedMilliseconds));
                    if (FrameTimer.ElapsedMilliseconds > 0)
                        ZXForm.MainForm?.SetFPS(1000 / 20);// (int)FrameTimer.ElapsedMilliseconds);
                }

                
            }     
        }

        /// <summary>
        /// The main emulation loop
        /// Executes Z80 Opcodes until 1 frame has passed
        /// </summary>
        public void ProcessFrame()
        {
            for (;;)
            {
                // Set z80 emulation speed     

                // Handle re-trig interrupts
                if (_cpu.IFF1 && (_cpu.EI_pending == 0) && (_cpu.TotalExecutedCycles < Machine.InterruptPeriod))
                {
                    GenerateInterrupt();
                }

                // Execute one Z80 instruction
                if (_cpu.TotalExecutedCycles > 4000)
                {
                    // check pending keyport update

                    // NMI
                }

                _cpu.ExecuteOne();                

                // Check whether frame has finished
                if (_cpu.TotalExecutedCycles >= Machine.FrameLength)
                {
                    // reset the total executed cycles counter
                    _cpu.TotalExecutedCycles -= Machine.FrameLength;

                    FrameCount++;

                    // approximately after every 15 frames the flash switches
                    if (FrameCount > 15)
                    {
                        FrameCount = 0;
                    }

                    // call the ULA update
                    _ULA.UpdateULA();

                    // update input

                    

                    // check whether an interrupt needs to be fired
                    _cpu.IFF1 = true;
                    if (_cpu.IFF1 && _cpu.EI_pending == 0)
                    {
                        //_cpu.R++;
                        GenerateInterrupt();
                        
                    }
                    
                    break;
                }
                _ULA.ProcessFrame();

                // fudge cycles for display
                _cpu.TotalExecutedCycles += (128 + 24 + 48 + 24) * 192;

            }
            
        }

        public void GenerateInterrupt()
        {
            //_cpu.NonMaskableInterrupt = true;
        }

        #endregion

    }
}
