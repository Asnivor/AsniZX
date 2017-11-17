using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsniZX.Emulation.Hardware.Sound.Beeper
{
    /// <summary>
    /// Respresents a high or low pulse of the ear bit
    /// </summary>
    public struct Pulse
    {
        /// <summary>
        /// True=High, False=Low
        /// </summary>
        public bool EarBit;

        /// <summary>
        /// Lenght of the pulse (given in Z80 tacts)
        /// </summary>
        public int Lenght;
    }
}
