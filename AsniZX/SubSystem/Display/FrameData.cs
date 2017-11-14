using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsniZX.SubSystem.Display
{
    public class FrameData
    {
        /// <summary>
        /// Frame width
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// Frame height
        /// </summary>
        public int Height { get; set; }
        /// <summary>
        /// Display Buffer
        /// </summary>
        public uint[] Buffer { get; set; }
        public byte[] BufferBytes { get; set; }
        /// <summary>
        /// The current spectrum border color
        /// (This will form the background when in fullscreen mode)
        /// </summary>
        public int BorderColour { get; set; }
    }
}
