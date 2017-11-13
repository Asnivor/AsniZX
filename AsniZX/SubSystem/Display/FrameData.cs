using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsniZX.SubSystem.Display
{
    public class FrameData
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public uint[] Buffer { get; set; }
        public byte[] BufferBytes { get; set; }
    }
}
