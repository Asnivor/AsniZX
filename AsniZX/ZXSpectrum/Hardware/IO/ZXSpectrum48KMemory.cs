using AsniZX.ZXSpectrum.Abstraction;
using AsniZX.ZXSpectrum.Hardware.CPU;
using AsniZX.ZXSpectrum.Hardware.CPU.Abstraction.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsniZX.ZXSpectrum.Hardware.IO
{
    /// <summary>
    /// Memory device for the 48k spectrum
    /// </summary>
    public class ZXSpectrum48KMemory : IMemoryDevice
    {
        #region Private Fields

        /// <summary>
        /// Reference to the Z80 CPU
        /// </summary>
        private Z80Cpu _cpu;

        // screen - todo

        /// <summary>
        /// The machine model memory (48k ram + 16k rom)
        /// </summary>
        private byte[] _memory;

        #endregion

        #region Public Properties

        /// <summary>
        /// The spectrum class
        /// </summary>
        public ISpectrum Spec { get; private set; }

        #endregion

        #region Construction

        public ZXSpectrum48KMemory(ISpectrum spec)
        {
            // Setup
            Spec = spec;
            _cpu = spec.Cpu;

            // screen todo

            _memory = new byte[0x10000];    // 65536 bytes (64k total)
        }

        #endregion

        #region IMemoryDevice

        /// <summary>
        /// Reads the memory at the specified address
        /// </summary>
        /// <param name="addr">Memory address</param>
        /// <returns>Byte read from the memory</returns>
        public byte OnReadMemory(ushort addr)
        {
            var data = _memory[addr];

            if ((addr & 0xC000) == 0x4000)
            {
                // Address is in the RAM range (above 16k)
                // Apply contention if neccessary

                //todo
            }

            return data;
        }

        /// <summary>
        /// Sets the memory value at the specified address
        /// </summary>
        /// <param name="addr">Memory address</param>
        /// <param name="value">Memory value to write</param>
        /// <returns>Byte read from the memory</returns>
        public void OnWriteMemory(ushort addr, byte data)
        {
            // Check whether memory is ROM or RAM
            switch (addr & 0xC000)
            {
                case 0x0000:
                    // Do nothing - we cannot write to ROM
                    return;
                case 0x4000:
                    // Address is RAM - apply contention if neccessary
                    break;
            }

            _memory[addr] = data;
        }

        /// <summary>
        /// Gets the buffer that holds the memory data
        /// </summary>
        /// <returns></returns>
        public byte[] GetMemoryBuffer() => _memory;

        #endregion

        #region IDevice

        /// <summary>
        /// Resets this device
        /// </summary>
        public void Reset()
        {

        }

        #endregion

        #region Model Specific Methods

        /// <summary>
        /// Emulates the ULA reading memory
        /// </summary>
        /// <param name="addr"></param>
        /// <returns></returns>
        public byte OnULAReadMemory(ushort addr)
        {
            var data = _memory[(addr & 0x3FFF) + 0x4000];

            return data;
        }

        /// <summary>
        /// Fills the memory with a row buffer starting at the specified memory address
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="startAddr"></param>
        public void FillMemory(byte[] buffer, ushort startAddr)
        {
            buffer?.CopyTo(_memory, startAddr);
        }

        #endregion
    }
}
