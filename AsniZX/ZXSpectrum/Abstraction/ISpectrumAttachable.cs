using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsniZX.ZXSpectrum.Abstraction
{
    /// <summary>
    /// Classes that implement this interface are able to attach to an instance of ISpectrum
    /// </summary>
    public interface ISpectrumAttachable
    {
        /// <summary>
        /// Attaches this device to the ISpectrum class
        /// Works as a way to not have to pass the ISpectrum instance into every device 
        /// via constructor
        /// </summary>
        /// <param name="spec"></param>
        void OnAttached(ISpectrum spec);
    }
}
