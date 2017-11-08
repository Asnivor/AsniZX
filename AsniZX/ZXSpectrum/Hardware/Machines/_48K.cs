using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsniZX.ZXSpectrum
{
    public class _48K: ISpeccyModel
    {
        public _48K(Spectrum spec)
        {
            Spec = spec;
            ClockSpeed = 3.5;
            RamSize = 0xC000;           // 48k
            RomSize = 0x4000;           // 16k
            FrameLength = 69888;
            InterruptPeriod = 32;
            AttributeRows = 24;
            AttributeCols = 32;
            ScreenWidth = 256;
            ScreenHeight = 192;
            BorderLeft = 48;
            BorderTop = 48;
            BorderRight = 48;
            BorderBottom = 56;
            DisplayStart = 16384;
            DisplayLength = 6144;
            AttributeStart = 22528;
            AttributeLength = 768;

            ContentionTable = new byte[70930];
            FloatingBusTable = new short[70930];
            for (int f = 0; f < 70930; f++)
                FloatingBusTable[f] = -1;

            Attr = new short[DisplayLength];


            BorderColour = 7;

            ScanLineWidth = BorderLeft + ScreenWidth + BorderRight;

            RomPath = AppDomain.CurrentDomain.BaseDirectory + @"ROMs\48.rom";
            RamSize = 65536;

        }

        public override byte ReadMemory(ushort addr)
        {

            addr &= 0xffff;

            //Spec._cpu.TotalExecutedCycles += 3;

            int page = (addr) >> 13;
            int offset = (addr) & 0x1FFF;

            byte b = Spec._ram[addr];

            return b;
        }

        public override void WriteMemory(ushort addr, byte data)
        {
            addr &= 0xffff;
            byte b = (byte)(data & 0xff);

            //Spec._cpu.TotalExecutedCycles += 3;

            int page = (addr) >> 13;
            int offset = (addr) & 0x1FFF;

            if ((addr & 49152) == 16384)
            {
                // video memory - update screen buffer
                Spec._ULA.UpdateScreenBuffer(Spec._cpu.TotalExecutedCycles);
            }

            // write to memory
            Spec._ram[addr] = b;
        }

        public override byte ReadPort(ushort addr)
        {
            return new byte();
        }

        public override void WritePort(ushort addr, byte data)
        {
            // every even I/O address will address the ULA, but really only port 0xfe should be used to avoid problems
            // If this port is written to, bits have the following meaning:
            /*
        Bit   7   6   5   4   3   2   1   0
            +-------------------------------+
            |   |   |   | E | M |   Border  |
            +-------------------------------+

            The lowest three bits specify the border colour; a zero in bit 3 activates the MIC output, 
            whilst a one in bit 4 activates the EAR output and the internal speaker. 
            However, the EAR and MIC sockets are connected only by resistors, so activating one activates the other; 
            the EAR is generally used for output as it produces a louder sound. The upper two bits are unused.
             */

            // check whether the low bit is reset
            bool lowBitReset = ((addr & 0x01) == 0);

            // If this is an EVEN address then ULA must be updated
            if (lowBitReset)
            {
                // Pass the data byte to the ULA
                Spec._ULA.LastULAPortOutput = data;

                // Update the border colour
                BorderColour = data & Spec.BORDER_BIT;

            }
        }

        public override void IRQCallBack()
        {
            //Console.WriteLine("IRQ with vec {0} and cpu.InterruptMode {1}", _cpu.Regs[_cpu.I], _cpu.InterruptMode);
            Spec._cpu.FlagI = false;
        }

        public override void NMICallBack()
        {
            //Console.WriteLine("NMI");
            Spec._cpu.NonMaskableInterrupt = false;
        }

        public override void BuildContentionTable()
        {
            /*
            int t = contentionStartPeriod;
            while (t < contentionEndPeriod)
            {
                //for 128 t-states
                for (int i = 0; i < 128; i += 8)
                {
                    contentionTable[t++] = 6;
                    contentionTable[t++] = 5;
                    contentionTable[t++] = 4;
                    contentionTable[t++] = 3;
                    contentionTable[t++] = 2;
                    contentionTable[t++] = 1;
                    contentionTable[t++] = 0;
                    contentionTable[t++] = 0;
                }
                t += (TstatesPerScanline - 128); //24 tstates of right border + left border + 48 tstates of retrace
            }

            //build top half of tstateToDisp table
            //vertical retrace period
            for (t = 0; t < ActualULAStart; t++)
                tstateToDisp[t] = 0;

            //next 48 are actual border
            while (t < ActualULAStart + (TstateAtTop))
            {
                //border(24t) + screen (128t) + border(24t) = 176 tstates
                for (int g = 0; g < 176; g++)
                    tstateToDisp[t++] = 1;

                //horizontal retrace
                for (int g = 176; g < TstatesPerScanline; g++)
                    tstateToDisp[t++] = 0;
            }

            //build middle half of display
            int _x = 0;
            int _y = 0;
            int scrval = 2;
            while (t < ActualULAStart + (TstateAtTop) + (ScreenHeight * TstatesPerScanline))
            {
                //left border
                for (int g = 0; g < 24; g++)
                    tstateToDisp[t++] = 1;

                //screen
                for (int g = 24; g < 24 + 128; g++)
                {
                    //Map screenaddr to tstate
                    if (g % 4 == 0)
                    {
                        scrval = (((((_y & 0xc0) >> 3) | (_y & 0x07) | (0x40)) << 8)) | (((_x >> 3) & 0x1f) | ((_y & 0x38) << 2));
                        _x += 8;
                    }
                    tstateToDisp[t++] = (short)scrval;
                }
                _y++;

                //right border
                for (int g = 24 + 128; g < 24 + 128 + 24; g++)
                    tstateToDisp[t++] = 1;

                //horizontal retrace
                for (int g = 24 + 128 + 24; g < 24 + 128 + 24 + 48; g++)
                    tstateToDisp[t++] = 0;
            }

            int h = contentionStartPeriod + 3;
            while (h < contentionEndPeriod + 3)
            {
                for (int j = 0; j < 128; j += 8)
                {
                    floatingBusTable[h] = tstateToDisp[h + 2];                    //screen address
                    floatingBusTable[h + 1] = attr[(tstateToDisp[h + 2] - 16384)];  //attr address
                    floatingBusTable[h + 2] = tstateToDisp[h + 2 + 4];             //screen address + 1
                    floatingBusTable[h + 3] = attr[(tstateToDisp[h + 2 + 4] - 16384)]; //attr address + 1
                    h += 8;
                }
                h += TstatesPerScanline - 128;
            }

            //build bottom border
            while (t < ActualULAStart + (TstateAtTop) + (ScreenHeight * TstatesPerScanline) + (TstateAtBottom))
            {
                //border(24t) + screen (128t) + border(24t) = 176 tstates
                for (int g = 0; g < 176; g++)
                    tstateToDisp[t++] = 1;

                //horizontal retrace
                for (int g = 176; g < TstatesPerScanline; g++)
                    tstateToDisp[t++] = 0;
            }
            */
        }

    }
}
