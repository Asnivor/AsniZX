using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsniZX.ZXSpectrum.Abstraction
{
    /// <summary>
    /// Similar to IDevice, except this is for Provider classes rather than devices
    /// </summary>
    public interface IProvider
    {
        /// <summary>
        /// Provider can reset itself
        /// </summary>
        void Reset();

        /// <summary>
        /// The emulated spectrum model that hosts the provider
        /// </summary>
        ISpectrum Spec { get; }
    }
}
