
using AsniZX.Emulation.Hardware.IO;
using System;
using System.Text;

namespace AsniZX.Emulation.Models.ZXSpectrum48
{
    public partial class ZXSpectrum48
    {
        /// <summary>
        /// Handles port read/writes
        /// </summary>
        public class Port : PortDeviceBase
        {
            /// <summary>
            /// Reads the port with the specified address
            /// </summary>
            /// <param name="addr"></param>
            /// <returns></returns>
            public override byte OnReadPort(ushort addr)
            {
                // Every even I/O address will address the ULA
                // Ignore everything else
                if ((addr & 0x0001) != 0)
                    return 0xFF;

                // keyboard input   
                var pBits = _keyboardDevice.ReadKeyboardByte(addr);

                var earBit = _tapeDevice.GetEarBit(_cpu.Tacts);
                int bin = Convert.ToInt32("10111111", 2);

                if (!earBit)                
                    pBits = (byte)(pBits & bin);

                return pBits;



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
            public override void OnWritePort(ushort addr, byte data)
            {
                // Only even addresses address the ULA
                if ((addr & 0x0001) == 0)
                {
                    // border
                    _borderDevice.BorderColour = data & 0x07;

                    // sound
                    _beeperDevice.ProcessEarBitValue(false, (data & 0x10) != 0);

                    // tape
                    _tapeDevice.ProcessMicBit((data & 0x08) != 0);
                }
            }
        }
    }
}
