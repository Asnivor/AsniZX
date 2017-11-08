using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsniZX.ZXSpectrum.Hardware.CPU
{
    /// <summary>
    /// Represents an event that is related to a Z80 operation code
    /// </summary>
    public class Z80OperationCodeEventArgs : Z80EventArgs
    {
        /// <summary>
        /// Z80 operation code
        /// </summary>
        public byte OpCode { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.EventArgs" /> class.
        /// </summary>
        public Z80OperationCodeEventArgs(byte opCode, Z80Cpu cpu) : base(cpu)
        {
            OpCode = opCode;
        }
    }
}
