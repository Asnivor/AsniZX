using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsniZX.ZXSpectrum.Hardware.CPU
{
    /// <summary>
    /// 8-bit Register indexes
    /// </summary>
    /// <remarks>
    /// This enum defines indexes to access register values through the
    /// C# this[] operator.
    /// </remarks>
    public enum Reg16Index : byte
    {
        BC = 0,
        DE = 1,
        HL = 2,
        SP = 3
    }
}
