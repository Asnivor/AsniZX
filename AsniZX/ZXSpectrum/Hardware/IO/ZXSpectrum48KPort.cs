using AsniZX.ZXSpectrum.Abstraction;
using AsniZX.ZXSpectrum.Hardware.CPU;
using AsniZX.ZXSpectrum.Hardware.CPU.Abstraction.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsniZX.ZXSpectrum.Hardware.IO
{
    /// <summary>
    /// Represents the port device part of the ULA
    /// Pretty much all actual I/O goes through here
    /// </summary>
    public class ZXSpectrum48KPort : IPortDevice
    {
        #region Private Fields

        /// <summary>
        /// Reference to the Z80 CPU
        /// </summary>
        private Z80Cpu _cpu;

        // border
        // beeper
        // keyboard
        // tape

        #endregion

        #region Public Properties

        /// <summary>
        /// The spectrum class
        /// </summary>
        public ISpectrum Spec { get; private set; }

        #endregion

        #region Construction

        public ZXSpectrum48KPort(ISpectrum spec)
        {
            // Setup
            Spec = spec;
            _cpu = spec.Cpu;

            // border todo
            // beeper todo
            // keyboard todo
            // tape todo
        }

        #endregion

        #region IPortDevice

        /// <summary>
        /// Reads the port with the specified address
        /// </summary>
        /// <param name="addr"></param>
        /// <returns></returns>
        public byte OnReadPort(ushort addr)
        {
            // Every even I/O address will address the ULA
            // Ignore everything else
            if ((addr & 0x0001) != 0)
                return 0xFF;

            /*The lowest three bits specify the border colour; a zero in bit 3 activates the MIC output, 
             * whilst a one in bit 4 activates the EAR output and the internal speaker. 
             * However, the EAR and MIC sockets are connected only by resistors, so activating one activates the other; 
             * the EAR is generally used for output as it produces a louder sound. The upper two bits are unused.
            Bit   7   6   5   4   3   2   1   0
                +-------------------------------+
                |   |   |   | E | M |   Border  |
                +-------------------------------+
            */

            /*
            var portBits = _keyboardDevice.GetLineStatus((byte)(addr >> 8));
            var earBit = _tapeDevice.GetEarBit(_cpu.Tacts);
            if (!earBit)
            {
                portBits = (byte)(portBits & 0b1011_1111);
            }
            return portBits;
            */

            return new byte();
        }

        /// <summary>
        /// Sends a byte of data to the specified port address
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="data"></param>
        public void OnWritePort(ushort addr, byte data)
        {
            // Only even addresses address the ULA
            if ((addr & 0x0001) == 0)
            {
                // border

                // sound

                // tape
            }
        }

        #endregion

        #region IDevice

        /// <summary>
        /// Resets this device
        /// </summary>
        public void Reset()
        {

        }

        #endregion
    }
}
