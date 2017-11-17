
using AsniZX.Emulation.Hardware.IO;

namespace AsniZX.Emulation.Models.ZXSpectrum48
{
    public partial class ZXSpectrum48
    {
        /// <summary>
        /// Handles raising vblank interrupts
        /// </summary>
        public class Interrupt : InterruptBase
        {
            public Interrupt(int interruptTState)
                : base(interruptTState)
            {
                InterruptTState = interruptTState;
                Longest_Operation_TStates = 23;
            }
        }
    }
}
