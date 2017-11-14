using AsniZX.Emulation.Hardware.Display;
using AsniZX.Emulation.Hardware.Machine;
using AsniZX.Emulation.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsniZX.Emulation.Hardware.Memory
{
    /// <summary>
    /// Memory device abstract class that other memory classes inherit from
    /// </summary>
    public abstract class MemoryDeviceBase : IMemoryDevice, IZXBoundDevice
    {
        /// <summary>
        /// Reference to the Z80 CPU
        /// </summary>
        protected IZ80Cpu _cpu { get; set; }

        /// <summary>
        /// The main screen device
        /// </summary>
        protected ScreenBase _screenDevice {get;set;}

        /// <summary>
        /// The machine model memory (ram + rom)
        /// </summary>
        protected byte[] _memory { get; set; }

        /// <summary>
        /// The size to set the memory buffer for this model machine
        /// </summary>
        public int MemorySize { get; set; }

        #region IMemoryDevice

        /// <summary>
        /// Reads the memory at the specified address
        /// </summary>
        /// <param name="addr">Memory address</param>
        /// <returns>Byte read from the memory</returns>
        public virtual byte OnReadMemory(ushort addr)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the memory value at the specified address
        /// </summary>
        /// <param name="addr">Memory address</param>
        /// <param name="value">Memory value to write</param>
        /// <returns>Byte read from the memory</returns>
        public virtual void OnWriteMemory(ushort addr, byte data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the memory buffer
        /// </summary>
        /// <returns></returns>
        public byte[] GetMemoryBuffer() => _memory;

        /// <summary>
        /// The ULA reads the memory at the specified address
        /// </summary>
        /// <param name="addr">Memory address</param>
        /// <returns>Byte read from the memory</returns>
        /// <remarks>
        /// We need this device to emulate the contention for the screen memory
        /// between the CPU and the ULA.
        /// </remarks>
        public virtual byte OnULAReadMemory(ushort addr)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Fills up the memory from the specified buffer
        /// </summary>
        /// <param name="buffer">Contains the row data to fill up the memory</param>
        /// <param name="startAddress">Z80 memory address to start filling up</param>
        public virtual void FillMemory(byte[] buffer, ushort startAddress = 0)
        {
            buffer?.CopyTo(_memory, startAddress);
        }

        #endregion

        #region IZXBoundDevice

        public ZXBase HostZX { get; set; }
        public virtual void OnAttached(ZXBase hostZX)
        {
            HostZX = hostZX;
            _cpu = hostZX.Cpu;
            _screenDevice = hostZX.ScreenDevice;
            _memory = new byte[MemorySize];
        }

        #endregion

        #region IDevice

        /// <summary>
        /// Resets this device by filling the memory with 0xFF
        /// </summary>
        public virtual void Reset()
        {
            Random rnd = new Random();
            lock (_screenDevice)
            {
                for (var i = 0; i < _memory.Length; i++)
                {
                    //OnWriteMemory((ushort)i, 0xFF);
                    //_memory[i] = (byte)rnd.Next(byte.MaxValue);
                }
            }
            
        }

        #endregion
    }
}
