﻿
namespace AsniZX.ZXSpectrum.Hardware.CPU.Abstraction.Devices
{
    /// <summary>
    /// This interface represents the memory used by the Z80 CPU.
    /// </summary>
    public interface IMemoryDevice
    {
        /// <summary>
        /// Reads the memory at the specified address
        /// </summary>
        /// <param name="addr">Memory address</param>
        /// <returns>Byte read from the memory</returns>
        byte OnReadMemory(ushort addr);

        /// <summary>
        /// Sets the memory value at the specified address
        /// </summary>
        /// <param name="addr">Memory address</param>
        /// <param name="value">Memory value to write</param>
        /// <returns>Byte read from the memory</returns>
        void OnWriteMemory(ushort addr, byte value);

        /// <summary>
        /// Gets the buffer that holds memory data
        /// </summary>
        /// <returns></returns>
        byte[] GetMemoryBuffer();
    }
}
