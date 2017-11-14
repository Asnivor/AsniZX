using AsniZX.Emulation.Hardware.Machine;
using AsniZX.Emulation.Interfaces;
using AsniZX.SubSystem.Display;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsniZX.Emulation.Hardware.Display
{
    /// <summary>
    /// The base abstract screen class
    /// So far most of this has been lifted from Dotneteer's Spect.Net.SpectrumEmu
    /// https://github.com/Dotneteer/spectnetide
    /// </summary>
    public abstract class ScreenBase: IDevice, IFrameBoundDevice, IZXBoundDevice
    {
        /// <summary>
        /// Renders the actual display to the form handle
        /// </summary>
        public DisplayHandler displayHandler { get; set; }

        /// <summary>
        /// The standard spectrum ULA colour palette
        /// </summary>
        public static List<uint> ULAColours = new List<uint>
        {
            0xFF000000, // Black
            0xFF0000AA, // Blue
            0xFFAA0000, // Red
            0xFFAA00AA, // Magenta
            0xFF00AA00, // Green
            0xFF00AAAA, // Cyan
            0xFFAAAA00, // Yellow
            0xFFAAAAAA, // White
            0xFF000000, // Bright Black
            0xFF0000FF, // Bright Blue
            0xFFFF0000, // Bright Red
            0xFFFF00FF, // Bright Magenta
            0xFF00FF00, // Bright Green
            0xFF00FFFF, // Bright Cyan
            0xFFFFFF00, // Bright Yellow
            0xFFFFFFFF, // Bright White
        };

        /// <summary>
        /// The previously instantiated ScreenConfig
        /// </summary>
        public ScreenConfig ScreenConfiguration { get; set; }

        protected byte[] _pixelBuffer;
        protected int[] _flashOffColors;
        protected int[] _flashOnColors;

        /// <summary>
        /// Handles the border colour
        /// </summary>
        protected BorderBase _borderDevice;

        /// <summary>
        /// Defines the action that accesses the screen memory
        /// </summary>
        protected Func<ushort, byte> _fetchScreenMemory;

        /// <summary>
        /// Table of ULA TState action information entries
        /// </summary>
        protected RenderingTState[] _renderingTStateTable;

        /// <summary>
        /// The current flash phase (normal/invert)
        /// </summary>
        protected bool _flashPhase;

        /// <summary>
        /// Pixel and attribute info stored while rendering the screen
        /// </summary>
        protected byte _pixelByte1;
        protected byte _pixelByte2;
        protected byte _attrByte1;
        protected byte _attrByte2;
        protected int _xPos;
        protected int _yPos;
        protected int _screenWidth;

        /// <summary>
        /// Initializes the ULA TState table
        /// </summary>
        protected virtual void InitializeUlaTStateTable()
        {
            // --- Reset the tact information table
            _renderingTStateTable = new RenderingTState[ScreenConfiguration.UlaFrameTStateCount];

            // --- Iterate through tacts
            for (var tact = 0; tact < ScreenConfiguration.UlaFrameTStateCount; tact++)
            {
                // --- We can put a tact shift logic here in the future
                // ...

                // --- calculate screen line and tact in line values here
                var line = tact / ScreenConfiguration.ScreenLineTime;
                var tactInLine = tact % ScreenConfiguration.ScreenLineTime;

                // --- Default tact description
                var tactItem = new RenderingTState
                {
                    Phase = ScreenRenderingPhase.None,
                    ContentionDelay = 0
                };

                if (ScreenConfiguration.IsTactVisible(line, tactInLine))
                {
                    // --- Calculate the pixel positions of the area
                    tactItem.XPos = (ushort)((tactInLine - ScreenConfiguration.HorizontalBlankingTime) * 2);
                    tactItem.YPos = (ushort)(line - ScreenConfiguration.VerticalSyncLines - ScreenConfiguration.NonVisibleBorderTopLines);

                    // --- The current tact is in a visible screen area (border or display area)
                    if (!ScreenConfiguration.IsTactInDisplayArea(line, tactInLine))
                    {
                        // --- Set the current border color
                        tactItem.Phase = ScreenRenderingPhase.Border;
                        if (line >= ScreenConfiguration.FirstDisplayLine && line <= ScreenConfiguration.LastDisplayLine)
                        {
                            // --- Left or right border area beside the display area
                            if (tactInLine == ScreenConfiguration.FirstPixelTStateInLine - ScreenConfiguration.PixelDataPrefetchTime)
                            {
                                // --- Fetch the first pixel data byte of the current line (2 tacts away)
                                tactItem.Phase = ScreenRenderingPhase.BorderAndFetchPixelByte;
                                tactItem.PixelByteToFetchAddress = CalculatePixelByteAddress(line, tactInLine + 2);
                                tactItem.ContentionDelay = 6;
                            }
                            else if (tactInLine == ScreenConfiguration.FirstPixelTStateInLine - ScreenConfiguration.AttributeDataPrefetchTime)
                            {
                                // --- Fetch the first attribute data byte of the current line (1 tact away)
                                tactItem.Phase = ScreenRenderingPhase.BorderAndFetchPixelAttribute;
                                tactItem.AttributeToFetchAddress = CalculateAttributeAddress(line, tactInLine + 1);
                                tactItem.ContentionDelay = 5;
                            }
                        }
                    }
                    else
                    {
                        // --- According to the tact, the ULA does separate actions
                        var pixelTact = tactInLine - ScreenConfiguration.FirstPixelTStateInLine;
                        switch (pixelTact & 7)
                        {
                            case 0:
                                // --- Display the current tact pixels
                                tactItem.Phase = ScreenRenderingPhase.DisplayByte1;
                                tactItem.ContentionDelay = 4;
                                break;
                            case 1:
                                // --- Display the current tact pixels
                                tactItem.Phase = ScreenRenderingPhase.DisplayByte1;
                                tactItem.ContentionDelay = 3;
                                break;
                            case 2:
                                // --- While displaying the current tact pixels, we need to prefetch the
                                // --- pixel data byte 2 tacts away
                                tactItem.Phase = ScreenRenderingPhase.DisplayByte1AndFetchByte2;
                                tactItem.PixelByteToFetchAddress = CalculatePixelByteAddress(line, tactInLine + 2);
                                tactItem.ContentionDelay = 2;
                                break;
                            case 3:
                                // --- While displaying the current tact pixels, we need to prefetch the
                                // --- attribute data byte 1 tacts away
                                tactItem.Phase = ScreenRenderingPhase.DisplayByte1AndFetchAttribute2;
                                tactItem.AttributeToFetchAddress = CalculateAttributeAddress(line, tactInLine + 1);
                                tactItem.ContentionDelay = 1;
                                break;
                            case 4:
                            case 5:
                                // --- Display the current tact pixels
                                tactItem.Phase = ScreenRenderingPhase.DisplayByte2;
                                break;
                            case 6:
                                if (tactInLine < ScreenConfiguration.FirstPixelTStateInLine + ScreenConfiguration.DisplayLineTime - 2)
                                {
                                    // --- There are still more bytes to display in this line.
                                    // --- While displaying the current tact pixels, we need to prefetch the
                                    // --- pixel data byte 2 tacts away
                                    tactItem.Phase = ScreenRenderingPhase.DisplayByte2AndFetchByte1;
                                    tactItem.PixelByteToFetchAddress = CalculatePixelByteAddress(line, tactInLine + 2);
                                    tactItem.ContentionDelay = 6;
                                }
                                else
                                {
                                    // --- Last byte in this line.
                                    // --- Display the current tact pixels
                                    tactItem.Phase = ScreenRenderingPhase.DisplayByte2;
                                }
                                break;
                            case 7:
                                if (tactInLine < ScreenConfiguration.FirstPixelTStateInLine + ScreenConfiguration.DisplayLineTime - 1)
                                {
                                    // --- There are still more bytes to display in this line.
                                    // --- While displaying the current tact pixels, we need to prefetch the
                                    // --- attribute data byte 1 tacts away
                                    tactItem.Phase = ScreenRenderingPhase.DisplayByte2AndFetchAttribute1;
                                    tactItem.AttributeToFetchAddress = CalculateAttributeAddress(line, tactInLine + 1);
                                    tactItem.ContentionDelay = 5;
                                }
                                else
                                {
                                    // --- Last byte in this line.
                                    // --- Display the current tact pixels
                                    tactItem.Phase = ScreenRenderingPhase.DisplayByte2;
                                }
                                break;
                        }
                    }
                }

                // --- Calculation is ready, let's store the calculated tact item
                _renderingTStateTable[tact] = tactItem;
            }
        }

        /// <summary>
        /// Calculates the pixel address for the specified line and tact within 
        /// the line
        /// </summary>
        /// <param name="line">Line index</param>
        /// <param name="tactInLine">Tacts index within the line</param>
        /// <returns>ZX spectrum screen memory address</returns>
        /// <remarks>
        /// Memory address bits: 
        /// C0-C2: pixel count within a byte -- not used in address calculation
        /// C3-C7: pixel byte within a line
        /// V0-V7: pixel line address
        /// 
        /// Direct Pixel Address (da)
        /// =================================================================
        /// |A15|A14|A13|A12|A11|A10|A9 |A8 |A7 |A6 |A5 |A4 |A3 |A2 |A1 |A0 |
        /// =================================================================
        /// | 0 | 0 | 0 |V7 |V6 |V5 |V4 |V3 |V2 |V1 |V0 |C7 |C6 |C5 |C4 |C3 |
        /// =================================================================
        /// | 1 | 1 | 1 | 1 | 1 | 0 | 0 | 0 | 0 | 0 | 0 | 1 | 1 | 1 | 1 | 1 | 0xF81F
        /// =================================================================
        /// | 0 | 0 | 0 | 0 | 0 | 1 | 1 | 1 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | 0x0700
        /// =================================================================
        /// | 0 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | 1 | 1 | 1 | 0 | 0 | 0 | 0 | 0 | 0x00E0
        /// =================================================================
        /// 
        /// Spectrum Pixel Address
        /// =================================================================
        /// |A15|A14|A13|A12|A11|A10|A9 |A8 |A7 |A6 |A5 |A4 |A3 |A2 |A1 |A0 |
        /// =================================================================
        /// | 0 | 0 | 0 |V7 |V6 |V2 |V1 |V0 |V5 |V4 |V3 |C7 |C6 |C5 |C4 |C3 |
        /// =================================================================
        /// </remarks>
        protected virtual ushort CalculatePixelByteAddress(int line, int tStateInLine)
        {
            var row = line - ScreenConfiguration.FirstDisplayLine;
            var column = 2 * (tStateInLine - (ScreenConfiguration.HorizontalBlankingTime + ScreenConfiguration.BorderLeftTime));
            var da = 0x4000 | (column >> 3) | (row << 5);
            return (ushort)((da & 0xF81F) // --- Reset V5, V4, V3, V2, V1
                | ((da & 0x0700) >> 3)    // --- Keep V5, V4, V3 only
                | ((da & 0x00E0) << 3));  // --- Exchange the V2, V1, V0 bit 
                                          // --- group with V5, V4, V3
        }

        /// <summary>
        /// Calculates the pixel attribute address for the specified line and 
        /// tact within the line
        /// </summary>
        /// <param name="line">Line index</param>
        /// <param name="tactInLine">Tacts index within the line</param>
        /// <returns>ZX spectrum screen memory address</returns>
        /// <remarks>
        /// Memory address bits: 
        /// C0-C2: pixel count within a byte -- not used in address calculation
        /// C3-C7: pixel byte within a line
        /// V0-V7: pixel line address
        /// 
        /// Spectrum Attribute Address
        /// =================================================================
        /// |A15|A14|A13|A12|A11|A10|A9 |A8 |A7 |A6 |A5 |A4 |A3 |A2 |A1 |A0 |
        /// =================================================================
        /// | 0 | 1 | 0 | 1 | 1 | 0 |V7 |V6 |V5 |V4 |V3 |C7 |C6 |C5 |C4 |C3 |
        /// =================================================================
        /// </remarks>
        protected virtual ushort CalculateAttributeAddress(int line, int tStateInLine)
        {
            var row = line - ScreenConfiguration.FirstDisplayLine;
            var column = 2 * (tStateInLine - (ScreenConfiguration.HorizontalBlankingTime + ScreenConfiguration.BorderLeftTime));
            var da = (column >> 3) | ((row >> 3) << 5);
            return (ushort)(0x5800 + da);
        }


        /// <summary>
        /// Fills the entire screen buffer with the specified data
        /// </summary>
        /// <param name="data">Data to fill the pixel buffer</param>
        public void FillScreenBuffer(byte data)
        {
            for (var i = 0; i < _pixelBuffer.Length; i++)
            {
                _pixelBuffer[i] = data;
            }
        }

        /// <summary>
        /// Executes the ULA rendering actions between the specified T-States
        /// </summary>
        /// <param name="fromTact">First ULA tact</param>
        /// <param name="toTact">Last ULA tact</param>
        public virtual void RenderScreen(int fromTact, int toTact)
        {
            // --- Adjust the tact boundaries
            fromTact = fromTact % ScreenConfiguration.UlaFrameTStateCount;
            toTact = toTact % ScreenConfiguration.UlaFrameTStateCount;

            // --- Carry out each tact action according to the rendering phase
            for (var currentTact = fromTact; currentTact <= toTact; currentTact++)
            {
                var ulaTact = _renderingTStateTable[currentTact];
                _xPos = ulaTact.XPos;
                _yPos = ulaTact.YPos;

                switch (ulaTact.Phase)
                {
                    case ScreenRenderingPhase.None:
                        // --- Invisible screen area, nothing to do
                        break;

                    case ScreenRenderingPhase.Border:
                        // --- Fetch the border color from ULA and set the corresponding border pixels
                        SetPixels(_borderDevice.BorderColour, _borderDevice.BorderColour);
                        break;

                    case ScreenRenderingPhase.BorderAndFetchPixelByte:
                        // --- Fetch the border color from ULA and set the corresponding border pixels
                        SetPixels(_borderDevice.BorderColour, _borderDevice.BorderColour);
                        // --- Obtain the future pixel byte
                        _pixelByte1 = _fetchScreenMemory(ulaTact.PixelByteToFetchAddress);
                        break;

                    case ScreenRenderingPhase.BorderAndFetchPixelAttribute:
                        // --- Fetch the border color from ULA and set the corresponding border pixels
                        SetPixels(_borderDevice.BorderColour, _borderDevice.BorderColour);
                        // --- Obtain the future attribute byte
                        _attrByte1 = _fetchScreenMemory(ulaTact.AttributeToFetchAddress);
                        break;

                    case ScreenRenderingPhase.DisplayByte1:
                        // --- Display bit 7 and 6 according to the corresponding color
                        SetPixels(
                            GetColor(_pixelByte1 & 0x80, _attrByte1),
                            GetColor(_pixelByte1 & 0x40, _attrByte1));
                        // --- Shift in the subsequent bits
                        _pixelByte1 <<= 2;
                        break;

                    case ScreenRenderingPhase.DisplayByte1AndFetchByte2:
                        // --- Display bit 7 and 6 according to the corresponding color
                        SetPixels(
                            GetColor(_pixelByte1 & 0x80, _attrByte1),
                            GetColor(_pixelByte1 & 0x40, _attrByte1));
                        // --- Shift in the subsequent bits
                        _pixelByte1 <<= 2;
                        // --- Obtain the next pixel byte
                        _pixelByte2 = _fetchScreenMemory(ulaTact.PixelByteToFetchAddress);
                        break;

                    case ScreenRenderingPhase.DisplayByte1AndFetchAttribute2:
                        // --- Display bit 7 and 6 according to the corresponding color
                        SetPixels(
                            GetColor(_pixelByte1 & 0x80, _attrByte1),
                            GetColor(_pixelByte1 & 0x40, _attrByte1));
                        // --- Shift in the subsequent bits
                        _pixelByte1 <<= 2;
                        // --- Obtain the next attribute
                        _attrByte2 = _fetchScreenMemory(ulaTact.AttributeToFetchAddress);
                        break;

                    case ScreenRenderingPhase.DisplayByte2:
                        // --- Display bit 7 and 6 according to the corresponding color
                        SetPixels(
                            GetColor(_pixelByte2 & 0x80, _attrByte2),
                            GetColor(_pixelByte2 & 0x40, _attrByte2));
                        // --- Shift in the subsequent bits
                        _pixelByte2 <<= 2;
                        break;

                    case ScreenRenderingPhase.DisplayByte2AndFetchByte1:
                        // --- Display bit 7 and 6 according to the corresponding color
                        SetPixels(
                            GetColor(_pixelByte2 & 0x80, _attrByte2),
                            GetColor(_pixelByte2 & 0x40, _attrByte2));
                        // --- Shift in the subsequent bits
                        _pixelByte2 <<= 2;
                        // --- Obtain the next pixel byte
                        _pixelByte1 = _fetchScreenMemory(ulaTact.PixelByteToFetchAddress);
                        break;

                    case ScreenRenderingPhase.DisplayByte2AndFetchAttribute1:
                        // --- Display bit 7 and 6 according to the corresponding color
                        SetPixels(
                            GetColor(_pixelByte2 & 0x80, _attrByte2),
                            GetColor(_pixelByte2 & 0x40, _attrByte2));
                        // --- Shift in the subsequent bits
                        _pixelByte2 <<= 2;
                        // --- Obtain the next attribute
                        _attrByte1 = _fetchScreenMemory(ulaTact.AttributeToFetchAddress);
                        break;
                }
            }
        }

        /// <summary>
        /// Gets the memory contention value for the specified T-State
        /// </summary>
        /// <param name="tact">ULA tact</param>
        /// <returns></returns>
        public byte GetContentionValue(int tState)
        {
            return _renderingTStateTable[tState % ScreenConfiguration.UlaFrameTStateCount].ContentionDelay;
        }

        /// <summary>
        /// Gets the buffer that holds the screen pixels
        /// </summary>
        /// <returns></returns>
        public byte[] GetPixelBuffer()
        {
            return _pixelBuffer;
        }

        /// <summary>
        /// Sets the two adjacent screen pixels belonging to the specified T-State to the given
        /// color
        /// </summary>
        /// <param name="colorIndex1">Color index of the first pixel</param>
        /// <param name="colorIndex2">Color index of the second pixel</param>
        protected void SetPixels(int colorIndex1, int colorIndex2)
        {
            var pos = _yPos * _screenWidth + _xPos;
            _pixelBuffer[pos++] = (byte)colorIndex1;
            _pixelBuffer[pos] = (byte)colorIndex2;
        }

        /// <summary>
        /// Gets the color index for the specified pixel value according
        /// to the given color attribute
        /// </summary>
        /// <param name="pixelValue">0 for paper pixel, non-zero for ink pixel</param>
        /// <param name="attr">Color attribute</param>
        /// <returns></returns>
        protected int GetColor(int pixelValue, byte attr)
        {
            var offset = (pixelValue == 0 ? 0 : 0x100) + attr;
            return _flashPhase
                ? _flashOnColors[offset]
                : _flashOffColors[offset];
        }


        /// <summary>
        /// No operation pixel renderer
        /// </summary>
        protected class NoopPixelRenderer// : VmComponentProviderBase, IScreenFrameProvider
        {
            /// <summary>
            /// The ULA signs that it's time to start a new frame
            /// </summary>
            public void StartNewFrame()
            {
            }

            /// <summary>
            /// Signs that the current frame is rendered and ready to be displayed
            /// </summary>
            /// <param name="frame">The buffer that contains the frame to display</param>
            public void DisplayFrame(byte[] frame)
            {
            }
        }


        #region IFrameBoundDevice

        /// <summary>
        /// Number of frames rendered
        /// </summary>
        public int FrameCount { get; set; }

        /// <summary>
        /// Number of T-States that have overflowed from the previous frame
        /// </summary>
        public int OverFlowTStates { get; set; }

        /// <summary>
        /// Allows the class to react to the start of a new frame
        /// </summary>
        public virtual void OnNewFrame()
        {
            FrameCount++;
            if (FrameCount % ScreenConfiguration.FlashToggleFrames == 0)
            {
                _flashPhase = !_flashPhase;
            }
            //_pixelRenderer?.StartNewFrame();
            RenderScreen(0, OverFlowTStates);
        }

        /// <summary>
        /// Once frame has completed the renderer must draw the frame
        /// </summary>
        public virtual void OnFrameCompleted()
        {
            // convert byte array to uint bitmap array
            var uints = new uint[_pixelBuffer.Length];
            for (int i = 0; i < _pixelBuffer.Length; i++)
            {
                uints[i] = ULAColours[_pixelBuffer[i] & 0x0F];  //(uint)_pixelBuffer[i];
            }

            var bt = BorderType.Original;

            int newScreenWidth;
            int newScreenHeight;
            uint[] translated;

            switch (bt)
            {
                case BorderType.None:
                    newScreenWidth = ScreenConfiguration.ScreenWidth - ScreenConfiguration.BorderLeftPixels - ScreenConfiguration.BorderRightPixels;
                    newScreenHeight = ScreenConfiguration.ScreenLines - ScreenConfiguration.BorderBottomLines - ScreenConfiguration.BorderTopLines;

                    // remove all of the borders
                    translated = new uint[newScreenWidth * newScreenHeight];

                    int index = 0;
                    int oldIndex = 0;

                    /// work through each display line
                    for (int line = 0; line <= ScreenConfiguration.DisplayLines; line++)
                    {
                        if (line < ScreenConfiguration.BorderTopLines || line > ScreenConfiguration.DisplayLines - ScreenConfiguration.BorderBottomLines)
                        {
                            // top and bottom border
                            oldIndex += ScreenConfiguration.ScreenWidth;
                            continue;
                        }

                        for (int pix = 0; pix <= ScreenConfiguration.ScreenWidth; pix++)
                        {
                            if (pix < ScreenConfiguration.BorderLeftPixels || pix > ScreenConfiguration.ScreenWidth - ScreenConfiguration.BorderRightPixels)
                            {
                                // left and right borders
                                oldIndex++;
                                continue;
                            }

                            translated[index] = uints[oldIndex];
                            index++;
                        }
                    }
                    
                    break;

                default:
                    translated = uints;
                    newScreenWidth = ScreenConfiguration.ScreenWidth;
                    newScreenHeight = ScreenConfiguration.ScreenLines;
                    break;
            }



            displayHandler?.UpdateDisplay
                (
                    new FrameData
                    {
                        Width = newScreenWidth,
                        Height = newScreenHeight,
                        Buffer = translated,
                        BufferBytes = _pixelBuffer,
                        BorderColour = HostZX.BorderDevice.BorderColour
                    }
                );
        }

        /// <summary>
        /// Allow external entities respond to frame completion
        /// </summary>
        public event EventHandler FrameCompleted;

        #endregion

        #region IZXBoundDevice

        public ZXBase HostZX { get; set; }
        public virtual void OnAttached(ZXBase hostZX)
        {
            HostZX = hostZX;
            displayHandler = hostZX.DHandler;
            _borderDevice = hostZX.BorderDevice;
            _fetchScreenMemory = hostZX.MemoryDevice.OnULAReadMemory;
            ScreenConfiguration = hostZX.ScreenConfiguration;
        }

        #endregion

        #region IDevice

        /// <summary>
        /// Resets this device
        /// </summary>
        public void Reset()
        {

        }

        #endregion
    }
}
