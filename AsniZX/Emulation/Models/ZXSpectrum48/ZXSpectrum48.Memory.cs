using AsniZX.Emulation.Hardware.Memory;

namespace AsniZX.Emulation.Models.ZXSpectrum48
{
    public partial class ZXSpectrum48
    {
        /// <summary>
        /// Handles memory read/writes
        /// </summary>
        public class Memory : MemoryDeviceBase
        {
            public Memory()
            {
                // set total memory size (ram + rom)
                MemorySize = 0x10000;       // 48k + 16k
            }

            /// <summary>
            /// Reads the memory at the specified address
            /// </summary>
            /// <param name="addr">Memory address</param>
            /// <returns>Byte read from the memory</returns>
            public override byte OnReadMemory(ushort addr)
            {
                var data = _memory[addr];
                if ((addr & 0xC000) == 0x4000)
                {
                    // Address is in the RAM range (above 16k)
                    // Apply contention if neccessary
                    _cpu.Delay(_screenDevice.GetContentionValue(HostZX.CurrentFrameTState));
                }
                return data;
            }

            /// <summary>
            /// Sets the memory value at the specified address
            /// </summary>
            /// <param name="addr">Memory address</param>
            /// <param name="value">Memory value to write</param>
            /// <returns>Byte read from the memory</returns>
            public override void OnWriteMemory(ushort addr, byte value)
            {
                // Check whether memory is ROM or RAM
                switch (addr & 0xC000)
                {
                    case 0x0000:
                        // Do nothing - we cannot write to ROM
                        return;
                    case 0x4000:
                        // Address is RAM - apply contention if neccessary
                        _cpu.Delay(_screenDevice.GetContentionValue(HostZX.CurrentFrameTState));
                        break;
                }
                _memory[addr] = value;
            }

            /// <summary>
            /// The ULA reads the memory at the specified address
            /// </summary>
            /// <param name="addr">Memory address</param>
            /// <returns>Byte read from the memory</returns>
            /// <remarks>
            /// We need this device to emulate the contention for the screen memory
            /// between the CPU and the ULA.
            /// </remarks>
            public override byte OnULAReadMemory(ushort addr)
            {
                var value = _memory[(addr & 0x3FFF) + 0x4000];
                return value;
            }


        }
    }
}
