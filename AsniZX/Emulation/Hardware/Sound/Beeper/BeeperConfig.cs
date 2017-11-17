using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsniZX.Emulation.Hardware.Sound.Beeper
{
    /// <summary>
    /// This class represents the parameters the Spectrum VM uses to render beeper
    /// and tape sound
    /// screen.
    /// </summary>
    public class BeeperConfig
    {
        /// <summary>
        /// The audio sample rate used to generate sound wave form
        /// </summary>
        /// <remarks>
        /// One ULA frame contains 69888 Z80 clock pulses.
        /// 69888 = 2^8 * 3 * 91
        /// Using 546 samples per frame (546 = 2 * 3 *91) we have exactly
        /// 128 (2^7) Z80 tacts in each sample.
        /// </remarks>
        public int AudioSampleRate { get; set; }

        /// <summary>
        /// The number of samples per ULA video frame
        /// </summary>
        public int SamplesPerFrame { get; set; }

        /// <summary>
        /// The number of ULA tacts per audio sample
        /// </summary>
        public int TStatesPerSample { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public BeeperConfig()
        {
            AudioSampleRate = 35000;
            SamplesPerFrame = 699;
            TStatesPerSample = 100;
        }
    }
}
