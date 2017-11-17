using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NAudio.Wave;
using System.Threading;
using NAudio.Wave.SampleProviders;

namespace AsniZX.SubSystem.Sound
{
    /// <summary>
    /// Takes all generated sound and plays it
    /// </summary>
    class SoundHandler
    {
        /// <summary>
        /// The static instance of this class (there can be only one)
        /// </summary>
        public static SoundHandler Instance;

        
        public AudioPlaybackEngine PlaybackEngine;

        /// <summary>
        /// The background thread that the sound runs on
        /// </summary>
        readonly Thread SoundThread;

        /// <summary>
        /// Whether the thread should be aborted or not
        /// </summary>
        public static bool AbortThread { private get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public SoundHandler()
        {
            AbortThread = false;
            PlaybackEngine = new AudioPlaybackEngine();
            SoundThread = new Thread(RunSound)
            {
                IsBackground = true,
                Priority = ThreadPriority.Normal
            };
            SoundThread.Start();
            
        }

        /// <summary>
        /// Initializes the soundhandler class
        /// </summary>
        public static void Init()
        {
            Instance = new SoundHandler();
        }

        public void RunSound()
        {
            //PlaybackEngine = new AudioPlaybackEngine();

            while (true)
            {
                // sound thread is running
            }
        }


        public void test()
        {
            //Stream
        }



        enum StreamingPlaybackState
        {
            Stopped,
            Playing,
            Buffering,
            Paused
        }
    }
}
