using AsniZX.Emulation.Hardware.Machine;
using AsniZX.Emulation.Hardware.Sound.Beeper;
using AsniZX.Emulation.Interfaces;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsniZX.SubSystem.Sound
{
    /// <summary>
    /// Renders the beeper pulses
    /// </summary>
    public class BeeperFrameProvider : ISampleProvider, IProvider
    {
        /// <summary>
        /// Number of sound frames buffered
        /// </summary>
        public const int FRAMES_BUFFERED = 50;
        public const int FRAMES_DELAYED = 2;

        public readonly BeeperConfig _beeperConfig;
        public float[] _waveBuffer;
        public int _bufferLength;
        public int _frameCount;
        public long _writeIndex;
        public long _readIndex;


        public AudioPlaybackEngine _PlaybackEngine;

        /// <summary>
        /// Gets the WaveFormat of this Sample Provider.
        /// </summary>
        /// <value>The wave format.</value>
        public WaveFormat WaveFormat { get; }        

        /// <summary>
        /// Init a new instance
        /// </summary>
        /// <param name="beeperConfig"></param>
        public BeeperFrameProvider(BeeperConfig beeperConfig)
        {
            _beeperConfig = beeperConfig;

            WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(beeperConfig.AudioSampleRate, 1);

            //_waveBuffer = new float[5600];

            // attach to the AudioPlaybackEngine
            _PlaybackEngine = SoundHandler.Instance.PlaybackEngine;

            _PlaybackEngine.AddBeeperInput(this);

            //Reset();
        }

        /// <summary>
        /// Adds the specified set of pulse samples to the sound
        /// </summary>
        /// <param name="samples">
        /// Array of sound samples (values between 0.0F and 1.0F)
        /// </param>
        public void AddSoundFrame(float[] samples)
        {
            // kludgy hack to remove the high frequency artifacts
            // need to look into this, its probably a fault in the code (or some kind of bug in RebelstarII)
            // Until then, if the entire sample array is made up of 1.0f, zero them out
            bool IsNoise = true;
            foreach (var s in samples)
                if (s == 0.0f)
                {
                    IsNoise = false;
                    break;
                }                               

            foreach (var sample in samples)
            {
                if (IsNoise == false)
                    _waveBuffer[_writeIndex++] = sample;
                else
                    _waveBuffer[_writeIndex++] = 0f;
                if (_writeIndex >= _bufferLength) _writeIndex = 0;
            }
        }

        /// <summary>
        /// Fill the specified buffer with 32 bit floating point samples
        /// </summary>
        /// <param name="buffer">The buffer to fill with samples.</param>
        /// <param name="offset">Offset into buffer</param>
        /// <param name="count">The number of samples to read</param>
        /// <returns>the number of samples written to the buffer.</returns>
        public int Read(float[] buffer, int offset, int count)
        {
            // --- We set up the initial buffer content for desired latency
            if (_frameCount <= FRAMES_DELAYED)
            {
                
                for (var i = 0; i < count; i++)
                {
                    buffer[offset++] = 0.0F;
                }
                
            }
            else
            {
                // --- We use the real samples
                for (var i = 0; i < count; i++)
                {
                    buffer[offset++] = _waveBuffer[_readIndex++];
                    if (_readIndex >= _bufferLength) _readIndex = 0;
                }
            }
            _frameCount++;
            return count;
        }

        public void PlaySound()
        {
            //SoundHandler.Instance.PlaybackEngine.PlaySound(this);
            _PlaybackEngine.PlaySound(this);
        }

        public void PauseSound()
        {
            SoundHandler.Instance.PlaybackEngine.PauseSound();
            _PlaybackEngine.PauseSound();
        }

        public void KillSound()
        {
            
        }

        #region IProvider

        public void Reset()
        {
            _bufferLength = (_beeperConfig.SamplesPerFrame + 1) * FRAMES_BUFFERED;
            _waveBuffer = new float[_bufferLength];
            _frameCount = 0;
            _writeIndex = 0;
            _readIndex = 0;
        }

        public ZXBase Spec { get; set; }
        public virtual void OnAttached(ZXBase hostZX)
        {
            Spec = hostZX;
            Reset();
        }

        #endregion
    }
}
