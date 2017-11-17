using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace AsniZX.SubSystem.Sound
{
    public class AudioPlaybackEngine : IDisposable
    {
        /// <summary>
        /// The default output device
        /// </summary>
        private readonly IWavePlayer outputDevice;
        /// <summary>
        /// Enables multiple sources to be played at the same time
        /// </summary>
        private readonly MixingSampleProvider mixer;
        /// <summary>
        /// The AudioPlaybackEngine sample rate
        /// </summary>
        private int SampleRate = 44100;// 35000; 
        /// <summary>
        /// Stereo Output
        /// </summary>
        private int OutputChannels = 2;

        /// <summary>
        /// The incoming Beeper feed
        /// </summary>
        private ISampleProvider BeeperInput;

        /// <summary>
        /// Main constructor - inits the mixer and sets it to 'play' constantly
        /// </summary>
        public AudioPlaybackEngine()
        {            
            // init the Mixer
            mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(SampleRate, OutputChannels));               
            // set the mixer to always return the number of samples requested by the Read() method
            mixer.ReadFully = true;
            outputDevice = new WaveOutEvent();
            ((WaveOutEvent)outputDevice as WaveOutEvent).DesiredLatency = 120;
            ((WaveOutEvent)outputDevice as WaveOutEvent).NumberOfBuffers = 3;
            
            outputDevice.Init(mixer);

            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = false;
                Thread.CurrentThread.Priority = ThreadPriority.Highest;
                // set playing
                outputDevice.Play();
            }).Start();
        }

        /// <summary>
        /// Adds a mono beeper output to a mixer input - converting it to 44100 stereo in the process
        /// </summary>
        /// <param name="input"></param>
        public void AddBeeperInput(ISampleProvider input)
        {
            // make sure the sample rate is 44100 and convert to stereo
            var resampStage = Ensure44100(input);
            // init volume stage
            var volumeStage = new VolumeSampleProvider(resampStage);
            // save to field
            BeeperInput = volumeStage;
            // add to the mixer
            mixer.AddMixerInput(BeeperInput);
        }

        /// <summary>
        /// Sets the Beeper volume (0.0f - 1.0f)
        /// </summary>
        /// <param name="atten"></param>
        public void SetBeeperVolume(float atten)
        {
            // handle out-of-range values
            if (atten < 0f)
                atten = 0f;
            if (atten > 1f)
                atten = 1f;

            // set the volume
            (BeeperInput as VolumeSampleProvider).Volume = atten;
        }

        public void PlaySound(ISampleProvider input)
        {
            //AddMixerInput(input);
            if (outputDevice.PlaybackState != PlaybackState.Playing)
                outputDevice.Play();
        }

        public void PauseSound()
        {
            if (outputDevice.PlaybackState == PlaybackState.Playing)
                outputDevice.Pause();
        }

        /// <summary>
        /// If this is a mono input it will convert it to stereo
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private ISampleProvider EnsureStereo(ISampleProvider input)
        {
            if (input.WaveFormat.Channels == 2)
                return input;

            if (input.WaveFormat.Channels == 1)
                return new MonoToStereoSampleProvider(input);

            throw new NotImplementedException("Too many incoming channels detected");
        }

        /// <summary>
        /// Resamples an ISampleProvider input to 44100 stereo
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private ISampleProvider Ensure44100(ISampleProvider input)
        {
            var resampler = new WdlResamplingSampleProvider(input, 44100);
            return resampler.ToStereo();
        }


        #region IDisposable

        public void Dispose()
        {
            outputDevice.Dispose();
        }

        #endregion
    }
}
