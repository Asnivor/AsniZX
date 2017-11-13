using AsniZX.Emulation.Hardware.CPU.SpectnetideZ80;
using AsniZX.Emulation.Hardware.Machine;
using AsniZX.Emulation.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsniZX.Emulation.Hardware.IO
{
    /// <summary>
    /// Emulates the ULA raising a maskable interrupt every frame
    /// </summary>
    public abstract class InterruptBase : IDevice, IFrameBoundDevice, IZXBoundDevice
    {
        /// <summary>
        /// Reference to the Z80 CPU
        /// </summary>
        protected IZ80Cpu _cpu;

        /// <summary>
        /// The ULA T-State to raise the interrupt at
        /// </summary>
        public int InterruptTState { get; set; }

        /// <summary>
        /// Whether or not an interrupt has been raised in this frame
        /// </summary>
        public bool InterruptRaised { get; set; }

        /// <summary>
        /// Whether or not an interrupt has been revoked in this frame
        /// </summary>
        public bool InterruptRevoked { get; set; }


        public int Longest_Operation_TStates { get; set; }

        /// <summary>
        /// Checks whether an interrupt is due in the current frame
        /// and raises one if neccessary
        /// </summary>
        /// <param name="currentTState"></param>
        public virtual void CheckForInterrupt(int currentTState)
        {
            if (InterruptRevoked)
            {
                // the interrupt has already been handled in this frame
                return;
            }

            if (currentTState < InterruptTState)
            {
                // we have not yet reached the T-State where the interrupt should be fired
                return;
            }

            if (currentTState > InterruptTState + Longest_Operation_TStates)
            {
                // we have passed the T-State at which the interrupt should have been called
                // plus the length in T-States that the ULA would have been holding down the INT pin
                // Revoke the INT signal (emulating the ULA stopping holding INT low) - CPU may or may not have handled 
                // the interrupt already
                InterruptRevoked = true;
                _cpu.StateFlags &= Z80StateFlags.InvInt;
                return;
            }

            if (InterruptRaised)
            {
                // At this point an interrupt has been raised, not revoked, and the CPU has not handled it yet
                // We can assume that ULA is still holding the INT pin low
                return;
            }

            if (_cpu.IsInterruptBlocked)
            {
                // CPU is blocking a maskable interrupt - therefore do not raise one
                return;
            }

            // If we have gotten this far, its time to raise an interrupt
            InterruptRaised = true;
            _cpu.StateFlags |= Z80StateFlags.Int;
            FrameCount++;
        }

        #region Construction

        public InterruptBase(int interruptTState)
        {
            InterruptTState = InterruptTState;
        }

        #endregion


        #region IFrameBoundDevice

        /// <summary>
        /// Number of frames rendered
        /// </summary>
        public int FrameCount { get; set; }

        /// <summary>
        /// Number of T-States that have overflowed from the previous frame
        /// </summary>
        public int OverFlowTStates { get; set; }

        /// <summary>
        /// Allows the class to react to the start of a new frame
        /// </summary>
        public virtual void OnNewFrame()
        {
            InterruptRaised = false;
            InterruptRevoked = false;
        }

        /// <summary>
        /// Allows the class to react to the completion of a frame
        /// </summary>
        public virtual void OnFrameCompleted() { }

        /// <summary>
        /// Allow external entities respond to frame completion
        /// </summary>
        public event EventHandler FrameCompleted;

        #endregion

        #region IZXBoundDevice

        public ZXBase HostZX { get; set; }
        public void OnAttached(ZXBase hostZX)
        {
            HostZX = hostZX;
            _cpu = hostZX.Cpu;
            Reset();
        }

        #endregion

        #region IDevice

        /// <summary>
        /// Resets this device
        /// </summary>
        public void Reset()
        {
            InterruptRaised = false;
            InterruptRevoked = false;
        }

        #endregion
    }
}
