using AsniZX.Extensions;
using AsniZX.SubSystem.Display;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsniZX.ZXSpectrum
{
    /// <summary>
    /// Uncommitted Logic Array generates the display, handles tape and audio IO
    /// and handles keyboard input.
    /// </summary>
    public class ULA
    {
        /// <summary>
        /// Reference to the Spectrum class passed in via constructor
        /// </summary>
        public Spectrum Spec { get; set; }

        /// <summary>
        /// Holds the last received data byte written to port 0xfe
        /// </summary>
        public byte LastULAPortOutput { get; set; }

        private uint _bufferPos;
        public readonly uint[] RawBitmap; // = new uint[GameWidth * GameHeight];

        public int LastTState { get; set; }

        private int _totalWidth;
        private int _totalHeight;

        private int temp = 0;

        public bool NeedsPaint { get; set; }

        

        /// <summary>
        /// Tstate display mapping array
        /// </summary>
        public short[] tStateToDisplay { get; set; }

        private Random rnd = new Random();

        public ULA(Spectrum spec)
        {
            Spec = spec;
            RawBitmap = new uint[GetTotalWidth() * GetTotalHeight()];
            LastULAPortOutput = 0;
            BuildAttributeMap();
        }

        /// <summary>
        /// The regular speccy palette
        /// </summary>
        public uint[] NormalColors = {
                                             0x000000,            // Blacks
                                             0x0000C0,            // Red
                                             0xC00000,            // Blue
                                             0xC000C0,            // Magenta
                                             0x00C000,            // Green
                                             0x00C0C0,            // Yellow
                                             0xC0C000,            // Cyan
                                             0xC0C0C0,            // White
                                             0x000000,            // Bright Black
                                             0x0000F0,            // Bright Red
                                             0xF00000,            // Bright Blue
                                             0xF000F0,            // Bright Magenta
                                             0x00F000,            // Bright Green
                                             0x00F0F0,            // Bright Yellow
                                             0xF0F000,            // Bright Cyan
                                             0xF0F0F0             // Bright White
                                    };

        public uint[] ULAPlusColours = new uint[64] { 0x000000, 0x404040, 0xff0000,0xff6a00,0xffd800,0xb6ff00,0x4cff00,0x00ff21,
                                                    0x00ff90,0x00ffff,0x0094ff,0x0026ff,0x4800ff,0xb200ff,0xff00dc,0xff006e,
                                                    0xffffff,0x808080,0x7f0000,0x7f3300,0x7f6a00,0x5b7f00,0x267f00,0x007f0e,
                                                    0x007f46,0x007f7f,0x004a7f,0x00137f,0x21007f,0x57007f,0x7f006e,0x7f0037,
                                                    0xa0a0a0,0x303030,0xff7f7f,0xffb27f,0xffe97f,0xdaff7f,0xa5ff7f,0x7fff8e,
                                                    0x7fffc5,0x7fffff,0x7fc9ff,0x7f92ff,0xa17fff,0xd67fff,0xff7fed,0xff7fb6,
                                                    0xc0c0c0,0x606060,0x7f3f3f,0x7f593f,0x7f743f,0x6d7f3f,0x527f3f,0x3f7f47,
                                                    0x3f7f62,0x3f7f7f,0x3f647f,0x3f497f,0x503f7f,0x6b3f7f,0x7f3f76,0x7f3f5b
                                                  };

        private int GetTotalWidth()
        {
            return Spec.Machine.ScanLineWidth;
        }

        private int GetTotalHeight()
        {
            return Spec.Machine.BorderTop + Spec.Machine.BorderBottom + Spec.Machine.ScreenHeight;
        }

        public void UpdateULA()
        {
            Spec.Machine.ULAByteCtr = 0;
            Spec.Machine.ScreenByteCtr = Spec.Machine.DisplayStart;
            LastTState = Spec.Machine.ULAStart;
            NeedsPaint = true;
        }

        public void UpdateScreenBuffer(int tStates)
        {
            if (tStates >= Spec.Machine.ULAStart)
            {
                return;
            }
            else if (tStates >= Spec.Machine.FrameLength)
            {
                tStates = Spec.Machine.FrameLength - 1;
                NeedsPaint = true;
            }

            // It takes 4 tstates to write 1 byte. Or, 2 pixels per t-state.
            int numBytes = (Spec._cpu.TotalExecutedCycles >> 2) + ((Spec._cpu.TotalExecutedCycles % 4) > 0 ? 1 : 0);

            int pixelData;
            int pixel2Data = 0xff;
            int attrData;
            int attr2Data;
            int bright;
            int ink;
            int paper;
            int flash;

        }

        public void BuildAttributeMap()
        {
            // build attribute map
            int start = Spec.Machine.DisplayStart;
            for (int f = 0; f < Spec.Machine.DisplayLength; f++, start++)
            {
                int addrH = start >> 8; //div by 256
                int addrL = start % 256;

                int pixelY = (addrH & 0x07);
                pixelY |= (addrL & (0xE0)) >> 2;
                pixelY |= (addrH & (0x18)) << 3;

                int attrIndex_Y = Spec.Machine.AttributeStart + ((pixelY >> 3) << 5);

                addrL = start % 256;
                int pixelX = addrL & (0x1F);

                Spec.Machine.Attr[f] = (short)(attrIndex_Y + pixelX);
            }
        }

        public void ProcessFrame()
        {
            RawBitmap.Fill(0u);

            _totalWidth = GetTotalWidth();
            _totalHeight = GetTotalHeight();

            // render borders - for now just render the whole screen space
            
            for (int row = 0; row < _totalHeight; row++)
            {
                for (int col = 0; col < _totalWidth; col++)
                {
                    RawBitmap[(row * _totalWidth) + col] = NormalColors[(uint)Spec.Machine.BorderColour];
                }
            }
            /*

            // fudge the scanline timing for now

            int offW = Spec.Machine.BorderLeft;
            int offH = Spec.Machine.BorderTop;

            // read attrib data
            byte[] attrDatas = new byte[768];
            Array.Copy(Spec._ram, Spec.Machine.AttributeStart, attrDatas, 0, Spec.Machine.AttributeLength);

            // populate bitmap with attrib
            int bright;
            int ink;
            int paper;
            int flash;

            


            //Spec.Machine.Attr;
            for (int a = 0; a < Spec.Machine.DisplayLength; a++)                
            {
                for (int i = 0; i < Spec.Machine.Attr.Length; i++)
                {

                }
                Spec.Machine.Attr[a] = 0;
            }

            for (int a = 0; a < Spec.Machine.AttributeLength; a++)
            {
                byte attrData = attrDatas[a];

                bright = (attrData & 0x40) >> 3;
                flash = (attrData & 0x80) >> 7;
                ink = (attrData & 0x07);
                paper = ((attrData >> 3) & 0x7);

                int paletteInk = (int)NormalColors[ink + bright];
                int palettePaper = (int)NormalColors[paper + bright];

                // now render pixels horizontally and vertically - each attrib block is 8x8
                for (int l = 0; l < 8; l++)
                {

                }

                // now render pixels vertically - each attrib block is 8x8
                for (int l = 0; l < 8; l++)
                {

                }
            }

            
            */


            FrameData fd = new FrameData();
            fd.Buffer = RawBitmap;
            fd.Width = _totalWidth;
            fd.Height = _totalHeight;

            ZXForm.MainForm.displayHandler.UpdateDisplay(fd);
        }

        public void ProcessFrame2()
        {
            byte pixelData;
            int pixel2Data = 0xff;
            byte attrData;
            int attr2Data;
            int bright;
            int ink;
            int paper;
            int flash;

            RawBitmap.Fill(0u);
            //_priority.Fill(0u);
            _bufferPos = 0;

            _totalHeight = GetTotalHeight();
            _totalWidth = GetTotalWidth();

            // get the video memory
            byte[] mem = new byte[0x4000];
            Array.Copy(Spec._ram, 0x0000 + temp, mem, 0, 0x4000);

            for (int i = 0; i < 0x4000; i += 2)
            {
                pixelData = mem[i];
                attrData = mem[i + 1];

                bright = (attrData & 0x40) >> 3;
                flash = (attrData & 0x80) >> 7;
                ink = (attrData & 0x07);
                paper = ((attrData >> 3) & 0x7);
                int paletteInk = (int)NormalColors[ink + bright];
                int palettePaper = (int)NormalColors[paper + bright];

                for (int a = 0; a < 8; ++a)
                {
                    if ((pixelData & 0x80) != 0)
                    {
                        RawBitmap[i] = (uint)paletteInk;
                    }
                    else
                    {
                        RawBitmap[i + 1] = (uint)palettePaper;
                    }
                }
         
            }

            /*
            while (_bufferPos < (_totalWidth * _totalHeight))
            {
                // 8x8
                var color = NormalColors[rnd.Next(0, NormalColors.Length - 1)];

                for (int i = 0; i < 64; i++)
                {
                    // horizontal
                    if (_bufferPos % 2 != 0)
                    {
                        ///RawBitmap[_bufferPos + (uint)i] = ULAPlusColours[rnd.Next(0, ULAPlusColours.Count() - 1)];
                    }
                    else
                    {
                        ///RawBitmap[_bufferPos + (uint)i] = ULAPlusColours[i];
                        RawBitmap[_bufferPos + (uint)i] = ULAPlusColours[rnd.Next(0, ULAPlusColours.Count() - 1)];
                    }

                }

                _bufferPos += 64;
            }
            */

            FrameData fd = new FrameData();
            fd.Buffer = RawBitmap;
            fd.Width = _totalWidth;
            fd.Height = _totalHeight;

            ZXForm.MainForm.displayHandler.UpdateDisplay(fd);

            temp++;
            if (temp > 0x4000)
                temp = 0;

        }
    }
}
