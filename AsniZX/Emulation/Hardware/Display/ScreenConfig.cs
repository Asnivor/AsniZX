using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsniZX.Emulation.Hardware.Display
{
    /// <summary>
    /// This class represents the parameters the ULA chip uses to render the Spectrum
    /// screen.
    /// </summary>
    public class ScreenConfig
    {
        /// <summary>
        /// Screen refresh rate per seconds
        /// </summary>
        public int RefreshRate { get; set; }

        /// <summary>
        /// The number of frames after the flash is toggled
        /// </summary>
        public int FlashToggleFrames { get; set; }

        /// <summary>
        /// Number of lines used for vertical synch
        /// </summary>
        public int VerticalSyncLines { get; set; }

        /// <summary>
        /// The number of top border lines that are not visible
        /// when rendering the screen
        /// </summary>
        public int NonVisibleBorderTopLines { get; set; }

        /// <summary>
        /// The number of border lines before the display
        /// </summary>
        public int BorderTopLines { get; set; }

        /// <summary>
        /// Number of display lines
        /// </summary>
        public int DisplayLines { get; set; }

        /// <summary>
        /// The number of border lines after the display
        /// </summary>
        public int BorderBottomLines { get; set; }

        /// <summary>
        /// The number of bottom border lines that are not visible
        /// when rendering the screen
        /// </summary>
        public int NonVisibleBorderBottomLines { get; set; }

        /// <summary>
        /// The total number of lines in the screen
        /// </summary>
        public int ScreenLines { get; set; }

        /// <summary>
        /// The first screen line that contains the top left display pixel
        /// </summary>
        public int FirstDisplayLine { get; set; }

        /// <summary>
        /// The last screen line that contains the bottom right display pixel
        /// </summary>
        public int LastDisplayLine { get; set; }

        /// <summary>
        /// The number of border pixels to the left of the display
        /// </summary>
        public int BorderLeftPixels { get; set; }

        /// <summary>
        /// The number of displayed pixels in a display row
        /// </summary>
        public int DisplayWidth { get; set; }

        /// <summary>
        /// The number of border pixels to the right of the display
        /// </summary>
        public int BorderRightPixels { get; set; }

        /// <summary>
        /// The total width of the screen in pixels
        /// </summary>
        public int ScreenWidth { get; set; }

        /// <summary>
        /// Horizontal blanking time (HSync+blanking).
        /// Given in Z80 clock cycles.
        /// </summary>
        public int HorizontalBlankingTime { get; set; }

        /// <summary>
        /// The time of displaying left part of the border.
        /// Given in Z80 clock cycles.
        /// </summary>
        public int BorderLeftTime { get; set; }

        /// <summary>
        /// The time of displaying a pixel row.
        /// Given in Z80 clock cycles.
        /// </summary>
        public int DisplayLineTime { get; set; }

        /// <summary>
        /// The time of displaying right part of the border.
        /// Given in Z80 clock cycles.
        /// </summary>
        public int BorderRightTime { get; set; }

        /// <summary>
        /// The time used to render the nonvisible right part of the border.
        /// Given in Z80 clock cycles.
        /// </summary>
        public int NonVisibleBorderRightTime { get; set; }

        /// <summary>
        /// The time of displaying a full screen line.
        /// Given in Z80 clock cycles.
        /// </summary>
        public int ScreenLineTime { get; set; }

        /// <summary>
        /// The time the data of a particular pixel should be prefetched
        /// before displaying it.
        /// Given in Z80 clock cycles.
        /// </summary>
        public int PixelDataPrefetchTime { get; set; }

        /// <summary>
        /// The time the data of a particular pixel attribute should be prefetched
        /// before displaying it.
        /// Given in Z80 clock cycles.
        /// </summary>
        public int AttributeDataPrefetchTime { get; set; }

        /// <summary>
        /// The tact within the line that should display the first pixel.
        /// Given in Z80 clock cycles.
        /// </summary>
        public int FirstPixelTStateInLine { get; set; }

        /// <summary>
        /// The tact in which the top left pixel should be displayed.
        /// Given in Z80 clock cycles.
        /// </summary>
        public int FirstDisplayPixelTState { get; set; }

        /// <summary>
        /// The tact in which the top left screen pixel (border) should be displayed
        /// </summary>
        public int FirstScreenPixelTState { get; set; }

        /// <summary>
        /// Defines the number of Z80 clock cycles used for the full rendering
        /// of the screen.
        /// </summary>
        public int UlaFrameTStateCount { get; set; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public ScreenConfig()
        {
            RefreshRate = 50;
            FlashToggleFrames = 25;
            VerticalSyncLines = 8;
            NonVisibleBorderTopLines = 8; // --- In a real screen this value is 0
            BorderTopLines = 48; // --- In a real screen this value is 55
            BorderBottomLines = 48; // --- In a real screen this value is 56
            NonVisibleBorderBottomLines = 8; // --- In a real screen this value is 0
            DisplayLines = 192;
            ScreenLines = BorderTopLines + DisplayLines + BorderBottomLines;
            FirstDisplayLine = VerticalSyncLines + NonVisibleBorderTopLines + BorderTopLines;
            LastDisplayLine = FirstDisplayLine + DisplayLines - 1;
            BorderLeftPixels = 48;
            BorderRightPixels = 48;
            DisplayWidth = 256;
            ScreenWidth = BorderLeftPixels + DisplayWidth + BorderRightPixels;
            HorizontalBlankingTime = 40;
            BorderLeftTime = 24;
            DisplayLineTime = 128;
            BorderRightTime = 24;
            NonVisibleBorderRightTime = 8;
            PixelDataPrefetchTime = 2;
            AttributeDataPrefetchTime = 1;
            FirstPixelTStateInLine = HorizontalBlankingTime + BorderLeftTime;
            ScreenLineTime = FirstPixelTStateInLine + DisplayLineTime + BorderRightTime + NonVisibleBorderRightTime;
            UlaFrameTStateCount = (FirstDisplayLine + DisplayLines + BorderBottomLines + NonVisibleBorderTopLines) *
                                ScreenLineTime;
            FirstDisplayPixelTState = FirstDisplayLine * ScreenLineTime
                + HorizontalBlankingTime + BorderLeftTime;
            FirstScreenPixelTState = (VerticalSyncLines + NonVisibleBorderTopLines) * ScreenLineTime
                + HorizontalBlankingTime;
        }

        /// <summary>
        /// Tests whether the specified tact is in the visible area of the screen.
        /// </summary>
        /// <param name="line">Line index</param>
        /// <param name="tactInLine">Tacts index within the line</param>
        /// <returns>
        /// True, if the tact is visible on the screen; otherwise, false
        /// </returns>
        public bool IsTactVisible(int line, int tactInLine)
        {
            var firstVisibleLine = VerticalSyncLines + NonVisibleBorderTopLines;
            var lastVisibleLine = firstVisibleLine + BorderTopLines + DisplayLines + BorderBottomLines;
            return
                line >= firstVisibleLine
                && line < lastVisibleLine
                && tactInLine >= HorizontalBlankingTime
                && tactInLine < ScreenLineTime - NonVisibleBorderRightTime;
        }

        /// <summary>
        /// Tests whether the tact is in the display area of the screen.
        /// </summary>
        /// <param name="line">Line index</param>
        /// <param name="tactInLine">Tacts index within the line</param>
        /// <returns>
        /// True, if the tact is within the display area of the screen; otherwise, false.
        /// </returns>
        public bool IsTactInDisplayArea(int line, int tactInLine)
        {
            return line >= FirstDisplayLine
                && line <= LastDisplayLine
                && tactInLine >= FirstPixelTStateInLine
                && tactInLine < FirstPixelTStateInLine + DisplayLineTime;
        }
    }
}