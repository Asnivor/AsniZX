using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsniZX.ZXSpectrum
{
    /// <summary>
    /// Different spectrum models inherit from this
    /// </summary>
    public abstract class ISpeccyModel
    {
        #region Properties

        /// <summary>
        /// Path on disk to the spectrum ROM
        /// </summary>
        public string RomPath { get; set; }

        #region Pointers

        /// <summary>
        /// The calling Spectrum class - passed in via constructor
        /// </summary>
        public Spectrum Spec { get; set; }

        #endregion

        #region Hardware

        /// <summary>
        /// Sepctrum ROM size
        /// </summary>
        public int RamSize { get; set; }

        /// <summary>
        ///  Spectrum ROM Size
        /// </summary>
        public int RomSize { get; set; }

        /// <summary>
        /// The frequency of the Z80A CPU (in MHz)
        /// </summary>
        public double ClockSpeed { get; set; }

        /// <summary>
        /// No. of TStates in 1 frame (after this interrupt is fired)
        /// </summary>
        public int FrameLength { get; set; }

        /// <summary>
        /// No. of TStates for interrupt (INT)
        /// </summary>
        public int InterruptPeriod { get; set; }

        #endregion

        #region ULA Properties

        /// <summary>
        /// No. of 8 pixel attribute screen blocks vertically & horizontally
        /// </summary>
        public int AttributeRows { get; set; }
        public int AttributeCols { get; set; }

        /// <summary>
        /// Width & height of the display area (not counting border) in pixels
        /// </summary>
        public int ScreenWidth { get; set; }
        public int ScreenHeight { get; set; }

        /// <summary>
        /// Border sizes in pixels
        /// </summary>
        public int BorderLeft { get; set; }
        public int BorderTop { get; set; }
        public int BorderRight { get; set; }
        public int BorderBottom { get; set; }

        /// <summary>
        /// Display start memory address
        /// </summary>
        public int DisplayStart { get; set; }

        /// <summary>
        /// No. of bytes of display memory
        /// </summary>
        public int DisplayLength { get; set; }

        /// <summary>
        /// Attribute start memory address
        /// </summary>
        public int AttributeStart { get; set; }

        /// <summary>
        /// No. of bytes of attribute memory
        /// </summary>
        public int AttributeLength { get; set; }

        /// <summary>
        /// No. of pixels in one scanline (BorderLeft + ScreenWidth + BorderRight)
        /// </summary>
        public int ScanLineWidth { get; set; }

        /// <summary>
        /// No. of T-States in 1 scanline
        /// </summary>
        public int TStatesPerScanLine { get; set; }

        /// <summary>
        /// The received border color
        /// </summary>
        public int BorderColour { get; set; }

        /// <summary>
        /// tstate of top left raster pixel
        /// </summary>
        public int ULAStart { get; set; }

        /// <summary>
        /// offset into display memory based on current tstate
        /// </summary>
        public int ScreenByteCtr;

        /// <summary>
        /// offset into current pixel of rasterizer
        /// </summary>
        public int ULAByteCtr;

        /// <summary>
        /// tstate-memory contention delay mapping
        /// </summary>
        public byte[] ContentionTable { get; set; }

        /// <summary>
        /// Floating bus
        /// </summary>
        public short[] FloatingBusTable { get; set; }


        public short[] Attr { get; set; }


        #endregion //ULA Properties

        #endregion //Properties



        #region CallBacks

        /// <summary>
        /// Read from a memory address
        /// </summary>
        /// <param name="addr"></param>
        /// <returns></returns>
        public virtual byte ReadMemory(ushort addr)
        {
            return new byte();
        }

        /// <summary>
        /// Write to memory address
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="value"></param>
        public virtual void WriteMemory(ushort addr, byte value)
        { }

        /// <summary>
        /// Read from a port
        /// </summary>
        /// <param name="addr"></param>
        /// <returns></returns>
        public virtual byte ReadPort(ushort addr)
        {
            return new byte();
        }

        /// <summary>
        /// Write to a port
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="value"></param>
        public virtual void WritePort(ushort addr, byte value)
        { }

        /// <summary>
        /// IRQ callback
        /// </summary>
        public virtual void IRQCallBack()
        { }

        /// <summary>
        /// NMI callback
        /// </summary>
        public virtual void NMICallBack()
        { }

        #endregion //CallBacks


        #region Other Model Specific Functions

        /// <summary>
        /// Builds the contention table for the 48k spectrum
        /// Taken almost verbatim from ArjunNair's Zero Spectrum emulator - https://github.com/ArjunNair/Zero-Emulator
        /// </summary>
        public virtual void BuildContentionTable()
        { }

        public virtual void HardReset()
        { }

        public virtual void ColdReset()
        {
            
        }

        #endregion
    }

    public enum SpecModel
    {
        _16k,
        _48k,
        _128K,
        _128Kplus,
        _128Kplus2,
        _128Kplus2A,
        _128kplus3
    }
}
