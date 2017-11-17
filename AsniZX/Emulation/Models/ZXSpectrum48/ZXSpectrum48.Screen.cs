
using AsniZX.Emulation.Hardware.Display;
using AsniZX.Emulation.Hardware.Machine;

namespace AsniZX.Emulation.Models.ZXSpectrum48
{
    public partial class ZXSpectrum48
    {
        /// <summary>
        /// Represents the ULA screen
        /// </summary>
        public class Screen : ScreenBase
        {
            public override void OnAttached(ZXBase hostZX)
            {
                base.OnAttached(hostZX);

                HostZX = hostZX;
                displayHandler = hostZX.DHandler;
                _borderDevice = hostZX.BorderDevice;
                _fetchScreenMemory = hostZX.MemoryDevice.OnULAReadMemory;
                ScreenConfiguration = hostZX.ScreenConfiguration;
                InitializeUlaTStateTable();
                _flashPhase = false;
                FrameCount = 0;

                // --- Calculate color conversion table
                _flashOffColors = new int[0x200];
                _flashOnColors = new int[0x200];

                for (var attr = 0; attr < 0x100; attr++)
                {
                    var ink = (attr & 0x07) | ((attr & 0x40) >> 3);
                    var paper = ((attr & 0x38) >> 3) | ((attr & 0x40) >> 3);
                    _flashOffColors[attr] = paper;
                    _flashOffColors[0x100 + attr] = ink;
                    _flashOnColors[attr] = (attr & 0x80) != 0 ? ink : paper;
                    _flashOnColors[0x100 + attr] = (attr & 0x80) != 0 ? paper : ink;
                }

                _screenWidth = hostZX.ScreenDevice.ScreenConfiguration.ScreenWidth;
                _pixelBuffer = new byte[_screenWidth * hostZX.ScreenDevice.ScreenConfiguration.ScreenLines];
            }
        }
    }
}
