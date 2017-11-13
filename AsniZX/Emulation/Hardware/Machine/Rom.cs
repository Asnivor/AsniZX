using AsniZX.Emulation.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsniZX.Emulation.Hardware.Machine
{
    /// <summary>
    /// Handles system ROMs
    /// </summary>
    public class Rom
    {
        /// <summary>
        /// The folder in which ROMs are stored
        /// </summary>
        public static string RomFolder = AppDomain.CurrentDomain.BaseDirectory +
            @"ROMs\";

        /// <summary>
        /// Loads a ROM into memory for the specified machine/device type
        /// </summary>
        /// <param name="machineType"></param>
        /// <returns></returns>
        public static bool LoadRom(MachineType machineType, IMemoryDevice memoryDevice)
        {
            bool success = false;
            string RomPath = null;            

            try
            {
                switch (machineType)
                {
                    case MachineType.ZXSpectrum48:
                        RomPath = RomFolder + "48.ROM";
                        byte[] _rom = System.IO.File.ReadAllBytes(RomPath);
                        memoryDevice.FillMemory(_rom);
                        success = true;
                        break;
                }
            }
            catch { }
            return success;
        }

        public static bool RomExists(string romPath)
        {
            if (System.IO.File.Exists(romPath))
                return true;

            return false;
        }
    }
}
