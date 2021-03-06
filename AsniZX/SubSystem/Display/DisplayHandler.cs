﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AsniZX.SubSystem.Display
{
    /// <summary>
    /// Main class for handling writing to the display
    /// </summary>
    public unsafe class DisplayHandler
    {
        /// <summary>
        /// Static reference to main form
        /// </summary>
        public ZXForm Form { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IRenderer _renderer;

        /// <summary>
        /// Dimensions of the frame coming from the emulator
        /// </summary>
        public int inputWidth = 352; //256;
        public int inputHeight = 288; // 192;

        /// <summary>
        /// Dimensions of the display surface that we are writing to
        /// </summary>
        int controlWidth, controlHeight;

        /// <summary>
        /// Framedata incoming buffer
        /// </summary>
        uint[] INbuffer;

        /// <summary>
        /// Initial window size (factor based on speccy output)
        /// </summary>
        public int magnification = 2;


        public DisplayHandler()
        {
            Form = ZXForm.MainForm;
            Init();
        }

        public void Init()
        {
            SetRenderer((IRenderer)Activator.CreateInstance(typeof(SharpDXRenderer)));
        }

        public void UpdateDisplay(FrameData frameData)
        {
            inputWidth = frameData.Width;
            inputHeight = frameData.Height;

            if (INbuffer == null || INbuffer.Length != (inputWidth * inputHeight))
            {
                // frame dimensions have changed
                INbuffer = new uint[inputWidth * inputHeight];
            }

            if (_renderer == null)
            {
                // renderer hasnt been inited yet
                Init();
            }

            
            // get the current control dimensions
            controlWidth = ((Control)_renderer as Control).ClientSize.Width;
            controlHeight = ((Control)_renderer as Control).ClientSize.Height;


            // draw
            _renderer.Draw(frameData);
        }

        private void SetRenderer(IRenderer renderer)
        {
            if (_renderer == renderer)
                return;

            if (_renderer != null)
            {
                var oldCtrl = (Control)renderer;
                _renderer.StopRendering();
                Form.Controls.Remove((Control)_renderer);
            }

            _renderer = renderer;
            var ctrl = (Control)_renderer;
            ctrl.Dock = DockStyle.Fill;
            ctrl.TabStop = false;
            Form.Controls.Add(ctrl);
            _renderer.Initialise(this);
        }

        /*
        public SharpDX.Mathematics.Interop.RawColor4 GetRawColor4(uint color)
        {
            byte a = (byte)(color >> 24);
            byte r = (byte)(color >> 16);
            byte g = (byte)(color >> 8);
            byte b = (byte)(color >> 0);
            return SharpDX.Color.FromAbgr(a, r, g, b);
        }
        */
    }
}
