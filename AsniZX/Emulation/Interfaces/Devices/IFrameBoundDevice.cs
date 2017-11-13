using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsniZX.Emulation.Interfaces
{
    /// <summary>
    /// Classes that implement this interface are bound to a rendering frame
    /// </summary>
    public interface IFrameBoundDevice : IDevice
    {
        /// <summary>
        /// Number of frames rendered
        /// </summary>
        int FrameCount { get; }

        /// <summary>
        /// Number of T-States that have overflowed from the previous frame
        /// </summary>
        int OverFlowTStates { get; set; }

        /// <summary>
        /// Allows the class to react to the start of a new frame
        /// </summary>
        void OnNewFrame();

        /// <summary>
        /// Allows the class to react to the completion of a frame
        /// </summary>
        void OnFrameCompleted();

        /// <summary>
        /// Allow external entities respond to frame completion
        /// </summary>
        event EventHandler FrameCompleted;
    }
}
