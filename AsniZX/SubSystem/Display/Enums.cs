using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsniZX.SubSystem.Display
{
    /// <summary>
    /// The various rendering engines AsniZX can use (currently only one)
    /// </summary>
    public enum RenderEngine
    {
        None,
        Direct3D,
        SharpDX,
        Software
    }

    /// <summary>
    /// Possible filter modes
    /// </summary>
    public enum FilterMode
    {
        NearestNeighbor,
        Linear
    }
}
