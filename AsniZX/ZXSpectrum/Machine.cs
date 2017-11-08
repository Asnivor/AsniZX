using AsniZX.ZXSpectrum.Abstraction;
using AsniZX.ZXSpectrum.Hardware.Machines;
using System;
using System.Threading;

namespace AsniZX.ZXSpectrum
{
    /// <summary>
    /// This is the over-arching class that controls the running of the emulator
    /// </summary>
    public class Machine
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
        /// Event should be fired everytime the Machine state changes
        /// Currently only used to update status in the UI
        /// </summary>
        public event EventHandler<MachineStateEventArgs> MachineStateChanged;

        /// <summary>
        /// Reference to the main form
        /// </summary>
        public ZXForm MainForm { get; set; }

        /// <summary>
        /// Spectrum model instance
        /// </summary>
        public ISpectrum Spectrum { get; private set; }

        public Machine()
        {
            // init the off state
            State = MachineState.Off;
            // fire first state change event
            MachineStateChanged?.Invoke(this, new MachineStateEventArgs(State, State));
            // reference the main window (form)
            MainForm = ZXForm.MainForm;

            // for the time being, we only have 48k model spectrum
            Spectrum = new ZXSpectrum48K(this);
            
        }

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

            if (MachineThread == null || MachineThread.IsAlive == false)
            {
                MachineThread = new Thread(new ThreadStart(BeginEmulation));
                MachineThread.Name = "Main Emulation Thread";
                MachineThread.Priority = ThreadPriority.Highest;
            }
        }

        /// <summary>
        /// Tells the machine to tat down the emulation
        /// </summary>
        public void Stop()
        {
            ChangeMachineState(MachineState.ShuttingDown);
            MachineThread.Abort();
            ChangeMachineState(MachineState.Off);
        }

        /// <summary>
        /// Pauses the emulation thread
        /// </summary>
        public void Pause()
        {
            ChangeMachineState(MachineState.Paused);
            MachineThread.Suspend();
        }

        /// <summary>
        /// Un-pauses the emulation thread
        /// </summary>
        public void UnPause()
        {
            ChangeMachineState(MachineState.Running);
            MachineThread.Resume();
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
