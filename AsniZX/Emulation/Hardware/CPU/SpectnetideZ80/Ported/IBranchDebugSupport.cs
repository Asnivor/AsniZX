using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsniZX.Emulation.Interfaces
{
    /// <summary>
    /// This interface provides information that support debugging branching statements
    /// </summary>
    public interface IBranchDebugSupport
    {
        /// <summary>
        /// Records a branching event
        /// </summary>
        /// <param name="ev">Event information</param>
        void RecordBranchEvent(BranchEvent ev);
    }
}
