using AsniZX.Emulation.Hardware.CPU.SpectnetideZ80;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AsniZX.Emulation.FileFormats.Snapshot
{
    /// <summary>
    /// Class that handles .sna snapshots
    /// </summary>
    public class SNA
    {
        /// <summary>
        /// The SNA snapshot type
        /// </summary>
        public SNA_Type Type { get; set; }

        /// <summary>
        /// Only one of the below will NOT be null - this is the one that should be used
        /// </summary>
        public SNA48 sna48 { get; set; }
        public SNA128_1 sna128_1 { get; set; }
        public SNA128_2 sna128_2 { get; set; }

        /// <summary>
        /// Identify whether the incoming filestream is a SNA file
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static bool IdentifySNA(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);

            //if (!((FileStream)stream as FileStream).Name.ToLower().EndsWith(".sna"))
                //return false;     
            
            if (stream.Length == 49179)
            {
                return true;
            }                
            else if (stream.Length == 131103)
            {
                return true;
            }                
            else if (stream.Length == 147487)
            {
                return true;
            }                
            else
            {
                // incorrect size
                return false;
            }
        }

        public static SNA LoadSNA(Stream stream)
        {
            SNA result = new SNA();

            stream.Seek(0, SeekOrigin.Begin);

            // 48k
            if (stream.Length == 49179)
            {
                byte[] hdr = new byte[49179];               
                stream.Read(hdr, 0, 49179);
                SNA48 sna48 = new SNA48();
                IntPtr hdrPtr = Marshal.AllocHGlobal(49179);
                Marshal.Copy(hdr, 0, hdrPtr, 49179);
                sna48 = (SNA48)Marshal.PtrToStructure(hdrPtr, typeof(SNA48));
                Marshal.FreeHGlobal(hdrPtr);

                result.Type = SNA_Type.SNA_48;
                result.sna48 = sna48;
            }

            // 128k_1
            if (stream.Length == 131103)
            {
                byte[] hdr = new byte[131103];
                stream.Read(hdr, 0, 131103);
                SNA128_1 sna128_1 = new SNA128_1();
                IntPtr hdrPtr = Marshal.AllocHGlobal(131103);
                Marshal.Copy(hdr, 0, hdrPtr, 131103);
                sna128_1 = (SNA128_1)Marshal.PtrToStructure(hdrPtr, typeof(SNA128_1));
                Marshal.FreeHGlobal(hdrPtr);

                result.Type = SNA_Type.SNA_128;
                result.sna128_1 = sna128_1;
            }

            // 128k_2
            if (stream.Length == 147487)
            {
                byte[] hdr = new byte[147487];
                stream.Read(hdr, 0, 147487);
                SNA128_2 sna128_2 = new SNA128_2();
                IntPtr hdrPtr = Marshal.AllocHGlobal(147487);
                Marshal.Copy(hdr, 0, hdrPtr, 147487);
                sna128_2 = (SNA128_2)Marshal.PtrToStructure(hdrPtr, typeof(SNA128_2));
                Marshal.FreeHGlobal(hdrPtr);

                result.Type = SNA_Type.SNA_128;
                result.sna128_2 = sna128_2;
            }

            return result;
        }

        /// <summary>
        /// Inject the snapshot into the emulated machine
        /// </summary>
        /// <param name="sna"></param>
        public static void InjectSnapshot(SNA sna)
        {
            var emu = ZXForm.MainForm.EmuMachine;
            var cpu = (Z80Cpu)emu.Spectrum.Cpu;
            switch (sna.Type)
            {
                case SNA_Type.SNA_48:

                    lock (emu.Spectrum.lockThis)
                    {

                        /*
                         * cpu.Registers.I = sna.sna48.I;
                        //cpu.Registers._HL_ = sna.sna48.HL_;
                        cpu.Registers._HL_ = (ushort)((sna.sna48.HL_ & 0xff) | (sna.sna48.HL_ >> 8));
                        cpu.Registers._DE_ = (ushort)((sna.sna48.DE_ & 0xff) | (sna.sna48.DE_ >> 8)); //sna.sna48.DE_;
                        cpu.Registers._BC_ = (ushort)((sna.sna48.BC_ & 0xff) | (sna.sna48.BC_ >> 8)); //sna.sna48.BC_;
                        cpu.Registers._AF_ = (ushort)((sna.sna48.AF_ & 0xff) | (sna.sna48.AF_ >> 8)); //sna.sna48.AF_;

                        cpu.Registers.HL = (ushort)((sna.sna48.HL & 0xff) | (sna.sna48.HL >> 8)); //sna.sna48.HL;
                        cpu.Registers.DE = (ushort)((sna.sna48.DE & 0xff) | (sna.sna48.DE >> 8)); //sna.sna48.DE;
                        cpu.Registers.BC = (ushort)((sna.sna48.BC & 0xff) | (sna.sna48.BC >> 8)); //sna.sna48.BC;
                        cpu.Registers.IY = (ushort)((sna.sna48.IY & 0xff) | (sna.sna48.IY >> 8)); //sna.sna48.IY;
                        cpu.Registers.IX = (ushort)((sna.sna48.IX & 0xff) | (sna.sna48.IX >> 8)); //sna.sna48.IX;

                        cpu.IFF1 = ((sna.sna48.IFF2 & 0x04) != 0);

                        if (cpu.IFF1)
                        {
                            cpu.BlockInterrupt();
                        }

                        cpu.Registers.R = sna.sna48.R;
                        cpu.Registers.AF = (ushort)((sna.sna48.AF & 0xff) | (sna.sna48.AF >> 8)); // sna.sna48.AF;
                        cpu.Registers.SP = (ushort)((sna.sna48.SP & 0xff) | (sna.sna48.SP >> 8)); // sna.sna48.SP;
                        */

                        cpu.Registers.I = sna.sna48.I;
                        cpu.Registers._HL_ = (ushort)(sna.sna48.HL_[0] + 256 * sna.sna48.HL_[1]);
                        cpu.Registers._DE_ = (ushort)(sna.sna48.DE_[0] + 256 * sna.sna48.DE_[1]); //sna.sna48.DE_;
                        cpu.Registers._BC_ = (ushort)(sna.sna48.BC_[0] + 256 * sna.sna48.BC_[1]); //sna.sna48.BC_;
                        cpu.Registers._AF_ = (ushort)(sna.sna48.AF_[0] + 256 * sna.sna48.AF_[1]); //sna.sna48.AF_;

                        cpu.Registers.HL = (ushort)(sna.sna48.HL[0] + 256 * sna.sna48.HL[1]); //sna.sna48.HL;
                        cpu.Registers.DE = (ushort)(sna.sna48.DE[0] + 256 * sna.sna48.DE[1]); //sna.sna48.DE;
                        cpu.Registers.BC = (ushort)(sna.sna48.BC[0] + 256 * sna.sna48.BC[1]); //sna.sna48.BC;
                        cpu.Registers.IY = (ushort)(sna.sna48.IY[0] + 256 * sna.sna48.IY[1]); ///sna.sna48.IY;
                        cpu.Registers.IX = (ushort)(sna.sna48.IX[0] + 256 * sna.sna48.IX[1]); //sna.sna48.IX;

                        cpu.IFF1 = cpu.IFF2 = (sna.sna48.IFF2 & 0x04) == 0x04;
                        //cpu.IFF1 = ((sna.sna48.IFF2 & 0x04) != 0);

                        if (cpu.IFF1)
                        {
                            cpu.BlockInterrupt();
                        }

                        cpu.Registers.R = sna.sna48.R;
                        cpu.Registers.AF = (ushort)(sna.sna48.AF[0] + 256 * sna.sna48.AF[1]); //sna.sna48.AF;
                        cpu.Registers.SP = (ushort)(sna.sna48.SP[0] + 256 * sna.sna48.SP[1]); //sna.sna48.SP;

                        cpu.SetInterruptMode(sna.sna48.IM);

                        emu.Spectrum.BorderDevice.BorderColour = sna.sna48.BorderColor;

                        //cpu.Registers.PC = emu.Spectrum.MemoryDevice.OnULAReadMemory(cpu.Registers.SP);
                        //cpu.Registers.SP += 2;

                        // Copy RAM to emulated machine
                        emu.Spectrum.MemoryDevice.FillMemory(sna.sna48.RAM, 0x4000);

                        //ushort tmp = emu.Spectrum.MemoryDevice.OnULAReadMemory(cpu.Registers.SP++);
                        //tmp |= (ushort)(emu.Spectrum.MemoryDevice.OnULAReadMemory(cpu.Registers.SP++) << 8);
                        //cpu.Registers.PC = tmp;
                    }

                    break;

                case SNA_Type.SNA_128:
                    throw new NotImplementedException();
            }
        }


        /// <summary>
        /// .SNA file layout - 48k - the header for both 48k & 128k
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct SNA48
        {
            /// <summary>
            /// I Register
            /// </summary>
            public byte I;
            /// <summary>
            /// Alt Registers
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] HL_, DE_, BC_, AF_;
            /// <summary>
            /// Main Registers (16bit)
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] HL, DE, BC, IY, IX;
            /// <summary>
            /// Interrupt (bit 2 contains IFF2, 1=EI/0=DI)
            /// </summary>
            public byte IFF2;
            /// <summary>
            /// R Register
            /// </summary>
            public byte R;
            /// <summary>
            /// AF Register
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] AF;
            /// <summary>
            /// SP Register
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] SP;
            /// <summary>
            /// IntMode (0=IM0/1=IM1/2=IM2)
            /// </summary>
            public byte IM;
            /// <summary>
            /// BorderColor (0..7, not used by Spectrum 1.7)
            /// </summary>
            public byte BorderColor;
            /// <summary>
            /// RAM dump 16384..65535
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 49152)]
            public byte[] RAM;
        } // 49179 bytes

        /// <summary>
        /// .SNA file layout - 128k - no duplicate paged banks
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct SNA128_1
        {
            /// <summary>
            /// I Register
            /// </summary>
            public byte I;
            /// <summary>
            /// Alt Registers
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] HL_, DE_, BC_, AF_;
            /// <summary>
            /// Main Registers (16bit)
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] HL, DE, BC, IY, IX;
            /// <summary>
            /// Interrupt (bit 2 contains IFF2, 1=EI/0=DI)
            /// </summary>
            public byte IFF2;
            /// <summary>
            /// R Register
            /// </summary>
            public byte R;
            /// <summary>
            /// AF Register
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] AF;
            /// <summary>
            /// SP Register
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] SP;
            /// <summary>
            /// IntMode (0=IM0/1=IM1/2=IM2)
            /// </summary>
            public byte IM;
            /// <summary>
            /// BorderColor (0..7, not used by Spectrum 1.7)
            /// </summary>
            public byte BorderColor;
            /// <summary>
            /// RAM Bank 5
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x4000)]
            public byte[] RAM_BANK5;
            /// <summary>
            /// RAM Bank 2
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x4000)]
            public byte[] RAM_BANK2;
            /// <summary>
            /// RAM Bank N (currently paged bank)
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x4000)]
            public byte[] RAM_BANKN;
            /// <summary>
            /// PC Register
            /// </summary>
            public ushort PC;
            /// <summary>
            /// Port 7ffd setting
            /// </summary>
            public byte Port0x7ffd;
            /// <summary>
            /// TR-DOS rom paged (1) or not (0)
            /// </summary>
            public byte TRDOS;
            /// <summary>
            /// Remaining 16k banks - 5 entries in ascending order (means no duplicate paged banks)
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x14000)]
            public byte[] RemainingRAMBanks;
        } // 131103 bytes


        /// <summary>
        /// .SNA file layout - 128k - duplicate paged banks
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct SNA128_2
        {
            /// <summary>
            /// I Register
            /// </summary>
            public byte I;
            /// <summary>
            /// Alt Registers
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] HL_, DE_, BC_, AF_;
            /// <summary>
            /// Main Registers (16bit)
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] HL, DE, BC, IY, IX;
            /// <summary>
            /// Interrupt (bit 2 contains IFF2, 1=EI/0=DI)
            /// </summary>
            public byte IFF2;
            /// <summary>
            /// R Register
            /// </summary>
            public byte R;
            /// <summary>
            /// AF Register
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] AF;
            /// <summary>
            /// SP Register
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] SP;
            /// <summary>
            /// IntMode (0=IM0/1=IM1/2=IM2)
            /// </summary>
            public byte IM;
            /// <summary>
            /// BorderColor (0..7, not used by Spectrum 1.7)
            /// </summary>
            public byte BorderColor;
            /// <summary>
            /// RAM Bank 5
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x4000)]
            public byte[] RAM_BANK5;
            /// <summary>
            /// RAM Bank 2
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x4000)]
            public byte[] RAM_BANK2;
            /// <summary>
            /// RAM Bank N (currently paged bank)
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x4000)]
            public byte[] RAM_BANKN;
            /// <summary>
            /// PC Register
            /// </summary>
            public ushort PC;
            /// <summary>
            /// Port 7ffd setting
            /// </summary>
            public byte Port0x7ffd;
            /// <summary>
            /// TR-DOS rom paged (1) or not (0)
            /// </summary>
            public byte TRDOS;
            /// <summary>
            /// Remaining 16k banks - 5 entries in ascending order (duplicate paged banks)
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x18000)]
            public byte[] RemainingRAMBanks;
        } // 147487 bytes

        public enum SNA_Type
        {
            SNA_48,
            SNA_128
        }
    }
}
