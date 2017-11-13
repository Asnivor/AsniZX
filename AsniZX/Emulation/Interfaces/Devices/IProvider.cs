using AsniZX.Emulation.Hardware.Machine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsniZX.Emulation.Interfaces
{
    public interface IProvider
    {
        /// <summary>
        /// The component provider should be able to reset itself
        /// </summary>
        void Reset();

        /// <summary>
        /// The virtual machine that hosts the provider
        /// </summary>
        ZXBase Spec { get; }

        /// <summary>
        /// Signs that the provider has been attached to the Spectrum virtual machine
        /// </summary>
        void OnAttached(ZXBase spec);
    }
}
