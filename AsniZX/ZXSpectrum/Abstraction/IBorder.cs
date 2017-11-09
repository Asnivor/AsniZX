﻿using AsniZX.ZXSpectrum.Hardware.CPU.Abstraction.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsniZX.ZXSpectrum.Abstraction
{
    /// <summary>
    /// ULA Border
    /// </summary>
    public interface IBorder : IDevice, ISpectrumAttachable
    {
        /// <summary>
        /// Gets/Sets the ULA border colour
        /// </summary>
        int BorderColour { get; set; }
    }
}
