using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsniZX.Emulation.Interfaces
{
    /// <summary>
    /// This interface defines the operations that support debugging the call stack
    /// </summary>
    public interface IStackDebugSupport
    {
        /// <summary>
        /// Resets the debug support
        /// </summary>
        void Reset();

        /// <summary>
        /// Records a stack pointer manipulation event
        /// </summary>
        /// <param name="ev">Event information</param>
        void RecordStackPointerManipulationEvent(StackPointerManipulationEvent ev);

        /// <summary>
        /// Records a stack content manipulation event
        /// </summary>
        /// <param name="ev">Event information</param>
        void RecordStackContentManipulationEvent(StackContentManipulationEvent ev);
    }
}
