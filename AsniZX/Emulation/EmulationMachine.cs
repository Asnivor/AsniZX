using AsniZX.Emulation.Hardware.Machine;
using AsniZX.SubSystem.Display;
using System;
using System.Threading;

namespace AsniZX.Emulation
{
    /// <summary>
    /// This is the over-arching class that controls the running of the emulator
    /// </summary>
    public class EmulationMachine
    {
        /// <summary>
        /// The current state of the machine
        /// </summary>
        public MachineState State { get; set; }

        /// <summary>
        /// The main emulation thread
        /// </summary>
        public Thread MachineThread { get; set; }

        /// <summary>
        /// Whether emulation is paused or not
        /// </summary>
        public bool IsPaused { get; set; }

        /// <summary>
        /// Emulation thread suspended
        /// </summary>
        public bool IsSuspended { get; set; }

        /// <summary>
        /// Event should be fired everytime the Machine state changes
        /// Currently only used to update status in the UI
        /// </summary>
        public event EventHandler<MachineStateEventArgs> MachineStateChanged;

        /// <summary>
        /// Reference to the main form (where the final rendered screen ends up)
        /// </summary>
        public ZXForm MainForm { get; set; }

        /// <summary>
        /// Spectrum model instance
        /// </summary>
        public ZXBase Spectrum { get; private set; }

        /// <summary>
        /// Renders the actual display
        /// </summary>
        public DisplayHandler displayHandler { get; set; }        

        public EmulationMachine()
        {
            // init the off state
            State = MachineState.Off;
            // fire first state change event
            MachineStateChanged?.Invoke(this, new MachineStateEventArgs(State, State));
            // reference the main window (form)
            MainForm = ZXForm.MainForm;
            displayHandler = MainForm.displayHandler;

            // for the time being, we only have 48k model spectrum
            Spectrum = new ZXSpectrum48(this);

            IsPaused = false;            
        }

        /// <summary>
        /// Thread handling
        /// </summary>
        public ManualResetEvent _shutdownEvent = new ManualResetEvent(false);
        public ManualResetEvent _pauseEvent = new ManualResetEvent(true);

        /// <summary>
        /// Tells the machine to start the emulation
        /// </summary>
        public void Start()
        {
            if (State != MachineState.Off)
            {
                // machine is not currently in the off state
                return;
            }

            // init the emulation
            ChangeMachineState(MachineState.Initialising);

            IsSuspended = false;

            MachineThread = new Thread(BeginEmulation);

            MachineThread.Start();
            ChangeMachineState(MachineState.Running);

            Console.WriteLine("Thread started running");
        }

        /// <summary>
        /// Pauses the emulation thread
        /// </summary>
        public void Pause()
        {
            if (IsSuspended)
                return;

            IsSuspended = true;
            
            IsPaused = true;

            MainForm.Pause();

            _pauseEvent.Reset();
            ChangeMachineState(MachineState.Paused);

            Console.WriteLine("Thread paused");
        }

        /// <summary>
        /// Un-pauses the emulation thread
        /// </summary>
        public void UnPause()
        {            
            IsPaused = false;
            IsSuspended = false;

            _pauseEvent.Set();
            ChangeMachineState(MachineState.Running);
            MainForm.UnPause();

            Console.WriteLine("Thread resuming ");
        }

        /// <summary>
        /// Tells the machine to tat down the emulation
        /// </summary>
        public void Stop()
        {
            ChangeMachineState(MachineState.ShuttingDown);

            // Signal the shutdown event
            _shutdownEvent.Set();

            // Make sure to resume any paused threads
            _pauseEvent.Set();

            // Wait for the thread to exit
            MachineThread.Join();

            
            //MachineThread?.Abort();
            ChangeMachineState(MachineState.Off);

            Console.WriteLine("Thread Stopped ");
        }


        

        private void ChangeMachineState(MachineState newState)
        {
            MachineState old = State;
            MachineState ne = newState;

            // Set the machine state
            State = newState;

            // Fire the change event
            MachineStateChanged?.Invoke(this, new MachineStateEventArgs(old, ne));
        }

        /// <summary>
        /// Called by the MachineThread
        /// </summary>
        private void BeginEmulation()
        {
            while (true)
            {
                bool res = Spectrum.ExecuteCycle(new CancellationToken());

                if (!res)
                    break;             
            }
        }
    }

    /// <summary>
    /// Various machine states that can exist
    /// </summary>
    public enum MachineState
    {
        Off = 0,
        Initialising = 1,
        Running = 2,
        Paused = 3,
        ShuttingDown = 4
    }

    /// <summary>
    /// Event arguments for machine state
    /// </summary>
    public class MachineStateEventArgs : EventArgs
    {
        /// <summary>
        /// Previous state
        /// </summary>
        public MachineState OldState { get; }

        /// <summary>
        /// New state
        /// </summary>
        public MachineState NewState { get; }

        /// <summary>
        /// Inits event arguments
        /// </summary>
        /// <param name="oldState"></param>
        /// <param name="newState"></param>
        public MachineStateEventArgs(MachineState oldState, MachineState newState)
        {
            OldState = oldState;
            NewState = newState;
        }
    }
}
