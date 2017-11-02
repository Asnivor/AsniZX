using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AsniZX.Extensions;
using AsniZX.SubSystem.Display;

using BizHawk.Z80A;

namespace AsniZX.Spectrum
{
    public class Emulator
    {
        // CPU
        Z80A _cpu;

        private Random rnd = new Random();

        private const int GameWidth = 256, GameHeight = 192;
        private uint _bufferPos;
        public readonly uint[] RawBitmap = new uint[GameWidth * GameHeight];
        /*
        private readonly uint[] _priority = new uint[GameWidth * GameHeight];
        private readonly uint[] _scanlineOAM = new uint[8 * 4];
        private readonly bool[] _isSprite0 = new bool[8];

        */

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

        public Emulator()
        {
            _cpu = new Z80A();
        }

        public void Start()
        {

        }

        public void ProcessFrame()
        {
            RawBitmap.Fill(0u);
            //_priority.Fill(0u);
            _bufferPos = 0;

            while (_bufferPos < (GameWidth * GameHeight))
            {
                // 8x8
                var color = NormalColors[rnd.Next(0, NormalColors.Length - 1)];

                for (int i = 0; i < 64; i++)
                {
                    
                    // horizontal
                    RawBitmap[_bufferPos + (uint)i] = ULAPlusColours[i];
                    // vertical
                    //RawBitmap[_bufferPos + (8 * (i))] = color;
                }

                _bufferPos += 64;

                /*
                if (_bufferPos % 2 != 0)
                {
                    RawBitmap[_bufferPos] = 0x000000;
                    
                }
                else
                {
                    //RawBitmap[_bufferPos] = NormalColors[rnd.Next(0, NormalColors.Length - 1)]; //(uint)rnd.Next(1 << 30);
                    RawBitmap[_bufferPos] = 0xC0C0C0;
                }

                _bufferPos++;
                */

            }

            FrameData fd = new FrameData();
            fd.Buffer = RawBitmap;
            fd.Width = GameWidth;
            fd.Height = GameHeight;

            ZXForm.MainForm.displayHandler.UpdateDisplay(fd);
                
        }
    }
}
