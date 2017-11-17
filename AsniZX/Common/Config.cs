using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsniZX.Common
{
    public class Config
    {
        public string SoundDevice { get; set; }
        public int SoundBufferSizeMs = 100;
        public bool SoundEnabled = true;
        public bool SoundThrottle = false;


        public Config()
        {
            SoundDevice = "XAudio2";
        }
    }
}
