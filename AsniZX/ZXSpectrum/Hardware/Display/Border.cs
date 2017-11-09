using AsniZX.ZXSpectrum.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsniZX.ZXSpectrum.Hardware.Display
{
    /// <summary>
    /// The border that the ULA uses
    /// </summary>
    public class Border : IBorder
    {
        #region Private Fields

        private int _borderColour;

        #endregion

        #region Public Properties

        /// <summary>
        /// The spectrum class
        /// </summary>
        public ISpectrum Spec { get; private set; }

        #endregion

        #region IBorder

        /// <summary>
        /// Gets/Sets the ULA border colour
        /// </summary>
        public int BorderColour
        {
            get { return _borderColour; }
            set { _borderColour = value & 0x07; }
        }

        #endregion

        #region ISpectrumAttachable

        public void OnAttached(ISpectrum spec)
        {
            Spec = spec;
        }

        #endregion

        #region IBorder

        /// <summary>
        /// Resets this device
        /// </summary>
        public void Reset()
        {
            BorderColour = 0;
        }

        #endregion
    }
}
