﻿using BizHawk.Emulation.Common;
using BizHawk.Common.NumberExtensions;

namespace AsniZX.ZXSpectrum
{
    public partial class Spectrum : IEmulator
    {
        public IEmulatorServiceProvider ServiceProvider { get; }

        public ControllerDefinition ControllerDefinition => null;

        public void FrameAdvance(IController controller, bool render, bool renderSound)
        {
            /*
            _controller = controller;
            _lagged = true;

            if (_tracer.Enabled)
            {
                _cpu.TraceCallback = s => _tracer.Put(s);
            }
            else
            {
                _cpu.TraceCallback = null;
            }

            _onPressed = controller.IsPressed("ON");

            if (_onPressed && ON_key_int_EN && !ON_key_int)
            {
                ON_key_int = true;
                _cpu.FlagI = true;
            }

            // see: http://wikiti.brandonw.net/index.php?title=83:Ports:04
            // for timer interrupt frequency

            // CPU frequency is 6MHz
            for (int i = 0; i < 100000; i++)
            {
                _cpu.ExecuteOne();

                TIM_count++;
                if (TIM_count >= TIM_hit)
                {
                    TIM_count = 0;

                    if (TIM_1_int_EN)
                    {
                        TIM_1_int = true;
                        _cpu.FlagI = true;
                    }
                }
            }

            Frame++;

            if (_lagged)
            {
                _lagCount++;
            }

            _isLag = _lagged;
            */
        }
		
        public int Frame => 0;

        public string SystemId => "Coleco";

        public bool DeterministicEmulation => true;

        public void ResetCounters()
        {
            //_frame = 0;
            _lagCount = 0;
            _isLag = false;
        }

        public CoreComm CoreComm { get; }
		
        public void Dispose()
        {
        }
    }
}
