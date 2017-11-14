using AsniZX.Emulation.Hardware.Display;
using AsniZX.Emulation.Hardware.Keyboard;
using AsniZX.Emulation.Hardware.Machine;
using AsniZX.Emulation.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsniZX.Emulation.Hardware.IO
{
    public abstract class PortDeviceBase : IPortDevice, IZXBoundDevice
    {
        /// <summary>
        /// Reference to the Z80 CPU
        /// </summary>
        protected IZ80Cpu _cpu { get; set; }

        protected BorderBase _borderDevice;
        protected KeyboardBase _keyboardDevice;
        //protected IBeeperDevice _beeperDevice;
        //protected IKeyboardDevice _keyboardDevice;
        //protected ITapeDevice _tapeDevice;

        // border
        // beeper
        // keyboard
        // tape

        #region IPortDevice

        /// <summary>
        /// Reads the port with the specified address
        /// </summary>
        /// <param name="addr"></param>
        /// <returns></returns>
        public virtual byte OnReadPort(ushort addr)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sends a byte of data to the specified port address
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="data"></param>
        public virtual void OnWritePort(ushort addr, byte data)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IZXBoundDevice

        public ZXBase HostZX { get; set; }
        public void OnAttached(ZXBase hostZX)
        {
            HostZX = hostZX;
            _cpu = hostZX.Cpu;
            _borderDevice = hostZX.BorderDevice;
            //_beeperDevice = hostVm.BeeperDevice;
            _keyboardDevice = hostZX.KeyboardDevice;
            //_tapeDevice = hostVm.TapeDevice;
        }

        #endregion

        #region IDevice

        /// <summary>
        /// Resets this device
        /// </summary>
        public virtual void Reset()
        {

        }

        #endregion
    }
}
