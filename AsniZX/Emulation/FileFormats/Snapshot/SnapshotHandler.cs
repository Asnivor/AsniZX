using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsniZX.Emulation.FileFormats.Snapshot
{
    /// <summary>
    /// Handles the loading and saving of snapshots
    /// </summary>
    public class SnapshotHandler
    {
        public static void LoadSnapshot(string path)
        {
            if (!File.Exists(path))
                return;

            // read the file to a bytearray
            byte[] data = File.ReadAllBytes(path);

            // convert to memorystream
            Stream stream = new MemoryStream(data);

            // identify snapshot type
            if (SNA.IdentifySNA(stream))
            {
                var snap = SNA.LoadSNA(stream);
                SNA.InjectSnapshot(snap);
                return;
            }
        }
    }
}
