using AsniZX.Emulation.Hardware.Machine;
using AsniZX.Emulation.Interfaces.Devices;
using AsniZX.SubSystem.Sound;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsniZX.Emulation.Hardware.Sound.Beeper
{
    /*
    public class Beeper : IBeeperDevice
    {

        public BeeperFrameProvider _beepFrameProvider { get; set; }
        private long _frameBegins;
        private int _frameTStates;
        private int _tStatesPerSample;
        private bool _useTapeMode;


        public Beeper(BeeperFrameProvider beepFrameProvider = null)
        {
            _beepFrameProvider = beepFrameProvider;
        }

        /// <summary>
        /// Get the beeper parameters
        /// </summary>
        public BeeperConfig BeeperConfiguration { get; private set; }

        /// <summary>
        /// The EAR bit pulses collected during the last frame
        /// </summary>
        public List<Pulse> Pulses { get; private set; }

        /// <summary>
        /// Gets the last value of the EAR bit
        /// </summary>
        public bool LastEarBit { get; private set; }

        /// <summary>
        /// Count of beeper frames since initialization
        /// </summary>
        public int FrameCount { get; private set; }

        /// <summary>
        /// Overflow from the previous frame, given in #of tacts 
        /// </summary>
        public int Overflow { get; set; }

        /// <summary>
        /// Gets the last pulse tact value
        /// </summary>
        public int LastPulseTState { get; private set; }

        /// <summary>
        /// Processes the change of the EAR bit value
        /// </summary>
        /// <param name="fromTape">
        /// False: EAR bit comes from an OUT instruction, 
        /// True: EAR bit comes from tape
        /// </param>
        /// <param name="earBit">EAR bit value</param>
        public void ProcessEarBitValue(bool fromTape, bool earBit)
        {
            if (!fromTape && _useTapeMode)
            {
                // --- The EAR bit comes from and OUT instruction, but now we're in tape mode
                return;
            }
            if (earBit == LastEarBit)
            {
                // --- The earbit has not changed
                return;
            }

            LastEarBit = earBit;
            var currentHostTState = HostZX.CurrentFrameTState;
            var currentTState = currentHostTState <= _frameTStates ? currentHostTState : _frameTStates;
            var length = currentTState - LastPulseTState;

            // --- If the first tact changes the pulse, we do
            // --- not add it
            if (length > 0)
            {
                Pulses.Add(new Pulse
                {
                    EarBit = !earBit,
                    Lenght = length
                });
            }
            LastPulseTState = currentTState;
        }

        /// <summary>
        /// This method signs that tape should override the OUT instruction's EAR bit
        /// </summary>
        /// <param name="useTape">
        /// True: Override the OUT instruction with the tape's EAR bit value
        /// </param>
        public void SetTapeOverride(bool useTape)
        {
            _useTapeMode = useTape;
        }

        /// <summary>
        /// Starts playing the sound
        /// </summary>
        public void PlaySound()
        {
            _beepFrameProvider?.PlaySound();
        }

        /// <summary>
        /// Pauses playing the sound
        /// </summary>
        public void PauseSound()
        {
            _beepFrameProvider?.PauseSound();
        }

        /// <summary>
        /// Stops playing the sound
        /// </summary>
        public void KillSound()
        {
            _beepFrameProvider?.KillSound();
        }

        /// <summary>
        /// Allow the device to react to the start of a new frame
        /// </summary>
        public void OnNewFrame()
        {
            Pulses.Clear();
            LastPulseTState = 0;
            FrameCount++;
        }




        #region IFrameBoundDevice

        /// <summary>
        /// Overflow from the previous frame, given in #of tacts 
        /// </summary>
        public int OverFlowTStates { get; set; }

        /// <summary>
        /// Allow external entities respond to frame completion
        /// </summary>
        public event EventHandler FrameCompleted;

        /// <summary>
        /// Allow the device to react to the completion of a frame
        /// </summary>
        public void OnFrameCompleted()
        {
            if (LastPulseTState <= _frameTStates - 1)
            {
                // --- We have to store the last pulse information
                Pulses.Add(new Pulse
                {
                    EarBit = LastEarBit,
                    Lenght = _frameTStates - LastPulseTState
                });
            }

            // --- Create the array for the samples
            var firstSampleOffset = _frameBegins % _tStatesPerSample == 0
                ? 0
                : _tStatesPerSample - (_frameBegins + _tStatesPerSample) % _tStatesPerSample;
            var samplesInFrame = (_frameTStates - firstSampleOffset - 1) / _tStatesPerSample + 1;
            var samples = new float[samplesInFrame];

            // --- Convert pulses to samples
            var sampleIndex = 0;
            var currentEnd = _frameBegins;

            foreach (var pulse in Pulses)
            {
                var firstSample = currentEnd % _tStatesPerSample == 0
                    ? currentEnd
                    : currentEnd + _tStatesPerSample - currentEnd % _tStatesPerSample;
                for (var i = firstSample; i < currentEnd + pulse.Lenght; i += _tStatesPerSample)
                {
                    samples[sampleIndex++] = pulse.EarBit ? 1.0F : 0.0F;
                }
                currentEnd += pulse.Lenght;
            }
            _beepFrameProvider?.AddSoundFrame(samples);
            _frameBegins += _frameTStates;
            //_beepFrameProvider?.PlaySound();
            PlaySound();
        }


        #endregion

        #region IZXBoundDevice

        public ZXBase HostZX { get; set; }
        public virtual void OnAttached(ZXBase hostZX)
        {
            HostZX = hostZX;
            BeeperConfiguration = new BeeperConfig();
            _beepFrameProvider = new BeeperFrameProvider(BeeperConfiguration);
            _frameTStates = hostZX.TStatesPerFrame;
            _tStatesPerSample = BeeperConfiguration.TStatesPerSample;
            Pulses = new List<Pulse>(1000);
            Reset();
        }

        #endregion

        #region IDevice

        /// <summary>
        /// Resets this device
        /// </summary>
        public void Reset()
        {
            Pulses.Clear();
            LastPulseTState = 0;
            LastEarBit = true;
            FrameCount = 0;
            _frameBegins = 0;
            _useTapeMode = false;
            _beepFrameProvider?.Reset();
        }

        #endregion
    }
    */
}
