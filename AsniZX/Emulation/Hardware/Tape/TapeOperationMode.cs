using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsniZX.Emulation.Hardware.Tape
{
    /// <summary>
    /// This enum represents the operation mode of the tape
    /// </summary>
    public enum TapeOperationMode : byte
    {
        /// <summary>
        /// The tape device is passive
        /// </summary>
        Passive = 0,

        /// <summary>
        /// The tape device is saving information (MIC pulses)
        /// </summary>
        Save,

        /// <summary>
        /// The tape device generates EAR pulses from a player
        /// </summary>
        Load
    }
}
