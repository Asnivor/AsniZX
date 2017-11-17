using System;
using System.Collections.Generic;


namespace AsniZX.Common
{
    public static class Global
    {
        public static Config Config;

        /// <summary>
		/// The maximum number of milliseconds the sound output buffer can go below full before causing a noticeable sound interruption.
		/// </summary>
		public static int SoundMaxBufferDeficitMs;

        public static int SoundSamplesPerFrame = 699;

        /// <summary>
		/// Used to disable secondary throttling (e.g. vsync, audio) for unthrottled modes or when the primary (clock) throttle is taking over (e.g. during fast forward/rewind).
		/// </summary>
		public static bool DisableSecondaryThrottling = false;
    }
}
