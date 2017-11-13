
namespace AsniZX.Emulation.Interfaces
{
    /// <summary>
    /// This interface represents the memory used by the Z80 CPU.
    /// </summary>
    public interface IMemoryDevice : IDevice, IZXBoundDevice
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

        /// <summary>
        /// The ULA reads the memory at the specified address
        /// </summary>
        /// <param name="addr">Memory address</param>
        /// <returns>Byte read from the memory</returns>
        /// <remarks>
        /// We need this device to emulate the contention for the screen memory
        /// between the CPU and the ULA.
        /// </remarks>
        byte OnULAReadMemory(ushort addr);

        /// <summary>
        /// Fills up the memory from the specified buffer
        /// </summary>
        /// <param name="buffer">Contains the row data to fill up the memory</param>
        /// <param name="startAddress">Z80 memory address to start filling up</param>
        void FillMemory(byte[] buffer, ushort startAddress = 0);
    }
}
