using AsniZX.Emulation.Hardware.Machine;
using AsniZX.Emulation.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsniZX.Emulation.Hardware.Display
{
    /// <summary>
    /// Base abstract class representing the machine border
    /// </summary>
    public abstract class BorderBase : IDevice, IZXBoundDevice
    {
        protected int _borderColour;

        /// <summary>
        /// Gets/Sets the border colour
        /// </summary>
        public virtual int BorderColour
        {
            get { return _borderColour; }
            set { _borderColour = value & 0x07; }
        }


        #region IZXBoundDevice

        public ZXBase HostZX { get; set; }
        public void OnAttached(ZXBase hostZX)
        {
            HostZX = hostZX;
        }

        #endregion

        #region IDevice

        /// <summary>
        /// Resets this device
        /// </summary>
        public virtual void Reset()
        {
            BorderColour = 0;
        }

        #endregion
    }
}
