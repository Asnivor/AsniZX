using AsniZX.Emulation.Hardware.Machine;
using AsniZX.Emulation.Interfaces;
using AsniZX.Emulation.Interfaces.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsniZX.Emulation.Hardware.Tape
{
    public abstract class TapeBase : IZXBoundDevice, ICpuOperationBoundDevice
    {
        private IZ80Cpu _cpu;
        private IBeeperDevice _beeperDevice;
        private TapeOperationMode _currentMode;
        private CommonTapeFilePlayer _tapePlayer;
        private long _lastMicBitActivityTact;
        private bool _micBitState;
        private SavePhase _savePhase;
        private int _pilotPulseCount;
        private int _bitOffset;
        private byte _dataByte;
        private int _dataLength;
        private byte[] _dataBuffer;
        private int _dataBlockCount;
        private MicPulseType _prevDataPulse;


        /// <summary>
        /// Number of tacts after save mod can be exited automatically
        /// </summary>
        public const int SAVE_STOP_SILENCE = 17500000;

        /// <summary>
        /// The address of the ERROR routine in the Spectrum ROM
        /// </summary>
        public const ushort ERROR_ROM_ADDRESS = 0x0008;

        /// <summary>
        /// The maximum distance between two scans of the EAR bit
        /// </summary>
        public const int MAX_TACT_JUMP = 10000;

        /// <summary>
        /// The width tolerance of save pulses
        /// </summary>
        public const int SAVE_PULSE_TOLERANCE = 24;

        /// <summary>
        /// Minimum number of pilot pulses before SYNC1
        /// </summary>
        public const int MIN_PILOT_PULSE_COUNT = 3000;

        /// <summary>
        /// Lenght of the data buffer to allocate for the SAVE operation
        /// </summary>
        public const int DATA_BUFFER_LENGTH = 0x10000;

        /// <summary>
        /// Gets the TZX tape content provider
        /// </summary>
        //public ITapeContentProvider ContentProvider { get; }

        /// <summary>
        /// Gets the TZX Save provider
        /// </summary>
        //public ISaveToTapeProvider SaveToTapeProvider { get; }


        /// <summary>
        /// Puts the device in save mode. From now on, every MIC pulse is recorded
        /// </summary>
        private void EnterSaveMode()
        {
            _currentMode = TapeOperationMode.Save;
            _savePhase = SavePhase.None;
            _micBitState = true;
            _lastMicBitActivityTact = _cpu.Tacts;
            _pilotPulseCount = 0;
            _prevDataPulse = MicPulseType.None;
            _dataBlockCount = 0;
            //SaveToTapeProvider?.CreateTapeFile();
        }

        /// <summary>
        /// Leaves the save mode. Stops recording MIC pulses
        /// </summary>
        private void LeaveSaveMode()
        {
            _currentMode = TapeOperationMode.Passive;
            //SaveToTapeProvider?.FinalizeTapeFile();
        }

        /// <summary>
        /// Puts the device in load mode. From now on, EAR pulses are played by a device
        /// </summary>
        private void EnterLoadMode()
        {
            _currentMode = TapeOperationMode.Load;

            //var contentReader = ContentProvider?.GetTapeContent();
            //if (contentReader == null) return;

            // --- Play the content
            //_tapePlayer = new CommonTapeFilePlayer(contentReader);
            //_tapePlayer.ReadContent();
            //_tapePlayer.InitPlay(_cpu.Tacts);
            HostZX.BeeperDevice.SetTapeOverride(true);
        }

        /// <summary>
        /// Leaves the load mode. Stops the device that playes EAR pulses
        /// </summary>
        private void LeaveLoadMode()
        {
            _currentMode = TapeOperationMode.Passive;
            _tapePlayer = null;
            //ContentProvider?.Reset();
            HostZX.BeeperDevice.SetTapeOverride(false);
        }

        /// <summary>
        /// Gets the EAR bit read from the tape
        /// </summary>
        /// <param name="cpuTicks"></param>
        /// <returns></returns>
        public virtual bool GetEarBit(long cpuTicks)
        {
            /*
            if (_currentMode != TapeOperationMode.Load)
            {
                return true;
            }
            var earBit = _tapePlayer?.GetEarBit(cpuTicks) ?? true;
            _beeperDevice.ProcessEarBitValue(true, earBit);
            return earBit;
            */

            return false;
        }

        /// <summary>
        /// Sets the current tape mode according to the current PC register
        /// and the MIC bit state
        /// </summary>
        public virtual void SetTapeMode()
        {
            switch (_currentMode)
            {
                case TapeOperationMode.Passive:
                    if (_cpu.Registers.PC == 10) //HostZX.RomInfo.LoadBytesRoutineAddress)
                    {
                        EnterLoadMode();
                    }
                    else if (_cpu.Registers.PC == 20)//HostZX.RomInfo.SaveBytesRoutineAddress)
                    {
                        EnterSaveMode();
                    }
                    return;
                case TapeOperationMode.Save:
                    if (_cpu.Registers.PC == ERROR_ROM_ADDRESS
                        || (int)(_cpu.Tacts - _lastMicBitActivityTact) > SAVE_STOP_SILENCE)
                    {
                        LeaveSaveMode();
                    }
                    return;
                case TapeOperationMode.Load:
                    /*
                    if ((_tapePlayer?.Eof ?? false) || _cpu.Registers.PC == ERROR_ROM_ADDRESS)
                    {
                        LeaveLoadMode();
                    }
                    */
                    return;
            }
        }

        /// <summary>
        /// Processes the the change of the MIC bit
        /// </summary>
        /// <param name="micBit"></param>
        public virtual void ProcessMicBit(bool micBit)
        {
            if (_currentMode != TapeOperationMode.Save
                || _micBitState == micBit)
            {
                return;
            }

            var length = _cpu.Tacts - _lastMicBitActivityTact;

            // --- Classify the pulse by its width
            var pulse = MicPulseType.None;
            if (length >= TapeDataBlockPlayer.BIT_0_PL - SAVE_PULSE_TOLERANCE
                && length <= TapeDataBlockPlayer.BIT_0_PL + SAVE_PULSE_TOLERANCE)
            {
                pulse = MicPulseType.Bit0;
            }
            else if (length >= TapeDataBlockPlayer.BIT_1_PL - SAVE_PULSE_TOLERANCE
                && length <= TapeDataBlockPlayer.BIT_1_PL + SAVE_PULSE_TOLERANCE)
            {
                pulse = MicPulseType.Bit1;
            }
            if (length >= TapeDataBlockPlayer.PILOT_PL - SAVE_PULSE_TOLERANCE
                && length <= TapeDataBlockPlayer.PILOT_PL + SAVE_PULSE_TOLERANCE)
            {
                pulse = MicPulseType.Pilot;
            }
            else if (length >= TapeDataBlockPlayer.SYNC_1_PL - SAVE_PULSE_TOLERANCE
                     && length <= TapeDataBlockPlayer.SYNC_1_PL + SAVE_PULSE_TOLERANCE)
            {
                pulse = MicPulseType.Sync1;
            }
            else if (length >= TapeDataBlockPlayer.SYNC_2_PL - SAVE_PULSE_TOLERANCE
                     && length <= TapeDataBlockPlayer.SYNC_2_PL + SAVE_PULSE_TOLERANCE)
            {
                pulse = MicPulseType.Sync2;
            }
            else if (length >= TapeDataBlockPlayer.TERM_SYNC - SAVE_PULSE_TOLERANCE
                     && length <= TapeDataBlockPlayer.TERM_SYNC + SAVE_PULSE_TOLERANCE)
            {
                pulse = MicPulseType.TermSync;
            }
            else if (length < TapeDataBlockPlayer.SYNC_1_PL - SAVE_PULSE_TOLERANCE)
            {
                pulse = MicPulseType.TooShort;
            }
            else if (length > TapeDataBlockPlayer.PILOT_PL + 2 * SAVE_PULSE_TOLERANCE)
            {
                pulse = MicPulseType.TooLong;
            }

            _micBitState = micBit;
            _lastMicBitActivityTact = _cpu.Tacts;

            // --- Lets process the pulse according to the current SAVE phase and pulse width
            var nextPhase = SavePhase.Error;
            switch (_savePhase)
            {
                case SavePhase.None:
                    if (pulse == MicPulseType.TooShort || pulse == MicPulseType.TooLong)
                    {
                        nextPhase = SavePhase.None;
                    }
                    else if (pulse == MicPulseType.Pilot)
                    {
                        _pilotPulseCount = 1;
                        nextPhase = SavePhase.Pilot;
                    }
                    break;
                case SavePhase.Pilot:
                    if (pulse == MicPulseType.Pilot)
                    {
                        _pilotPulseCount++;
                        nextPhase = SavePhase.Pilot;
                    }
                    else if (pulse == MicPulseType.Sync1 && _pilotPulseCount >= MIN_PILOT_PULSE_COUNT)
                    {
                        nextPhase = SavePhase.Sync1;
                    }
                    break;
                case SavePhase.Sync1:
                    if (pulse == MicPulseType.Sync2)
                    {
                        nextPhase = SavePhase.Sync2;
                    }
                    break;
                case SavePhase.Sync2:
                    if (pulse == MicPulseType.Bit0 || pulse == MicPulseType.Bit1)
                    {
                        // --- Next pulse starts data, prepare for receiving it
                        _prevDataPulse = pulse;
                        nextPhase = SavePhase.Data;
                        _bitOffset = 0;
                        _dataByte = 0;
                        _dataLength = 0;
                        _dataBuffer = new byte[DATA_BUFFER_LENGTH];
                    }
                    break;
                case SavePhase.Data:
                    if (pulse == MicPulseType.Bit0 || pulse == MicPulseType.Bit1)
                    {
                        if (_prevDataPulse == MicPulseType.None)
                        {
                            // --- We are waiting for the second half of the bit pulse
                            _prevDataPulse = pulse;
                            nextPhase = SavePhase.Data;
                        }
                        else if (_prevDataPulse == pulse)
                        {
                            // --- We received a full valid bit pulse
                            nextPhase = SavePhase.Data;
                            _prevDataPulse = MicPulseType.None;

                            // --- Add this bit to the received data
                            _bitOffset++;
                            _dataByte = (byte)(_dataByte * 2 + (pulse == MicPulseType.Bit0 ? 0 : 1));
                            if (_bitOffset == 8)
                            {
                                // --- We received a full byte
                                _dataBuffer[_dataLength++] = _dataByte;
                                _dataByte = 0;
                                _bitOffset = 0;
                            }
                        }
                    }
                    else if (pulse == MicPulseType.TermSync)
                    {
                        // --- We received the terminating pulse, the datablock has been completed
                        nextPhase = SavePhase.None;
                        _dataBlockCount++;

                        // --- Create and save the data block
                        /*
                        var dataBlock = new TzxStandardSpeedDataBlock
                        {
                            Data = _dataBuffer,
                            DataLength = (ushort)_dataLength
                        };
                        */

                        // --- If this is the first data block, extract the name from the header
                        if (_dataBlockCount == 1 && _dataLength == 0x13)
                        {
                            // --- It's a header!
                            var sb = new StringBuilder(16);
                            for (var i = 2; i <= 11; i++)
                            {
                                sb.Append((char)_dataBuffer[i]);
                            }
                            var name = sb.ToString().TrimEnd();
                            //SaveToTapeProvider?.SetName(name);
                        }
                        //SaveToTapeProvider?.SaveTapeBlock(dataBlock);
                    }
                    break;
            }
            _savePhase = nextPhase;
        }

        /// <summary>
        /// External entities can respond to the event when a fast load completed.
        /// </summary>
        public event EventHandler FastLoadCompleted;


        #region ICpuOperationBoundDevice

        /// <summary>
        /// Allow the device to react to the start of a new frame
        /// </summary>
        public void OnCpuOperationCompleted()
        {
            SetTapeMode();
            /*
            if (CurrentMode == TapeOperationMode.Load
                && HostZX.ExecuteCycleOptions.FastTapeMode
                && TapeFilePlayer != null
                && TapeFilePlayer.PlayPhase != PlayPhase.Completed
                && _cpu.Registers.PC == HostZX.RomInfo.LoadBytesRoutineAddress)
            {
                if (FastLoadFromTzx())
                {
                    FastLoadCompleted?.Invoke(this, EventArgs.Empty);
                }
            }
            */
        }

        #endregion

        #region IDevice

        public virtual void Reset()
        {
            //ContentProvider?.Reset();
            _tapePlayer = null;
            _currentMode = TapeOperationMode.Passive;
            _savePhase = SavePhase.None;
            _micBitState = true;
        }

        #endregion


        #region IZXBoundDevice

        public ZXBase HostZX { get; set; }
        public virtual void OnAttached(ZXBase hostZX)
        {
            HostZX = hostZX;
            _cpu = hostZX.Cpu;
            _beeperDevice = hostZX.BeeperDevice;
        }

        #endregion


        /// <summary>
        /// The current operation mode of the tape
        /// </summary>
        public TapeOperationMode CurrentMode => _currentMode;
    }
}
