using AsniZX.SubSystem.Display;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AsniZX
{
    public partial class ZXForm : Form
    {
        /// <summary>
        /// Quit out of the application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _rendererRunning = false;
            _renderThread?.Abort();
            Speccy.EmulationThread?.Abort();
            Application.Exit();
        }

        private void ZXForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _rendererRunning = false;
            _renderThread?.Abort();
            Speccy.EmulationThread?.Abort();
        }

        /*
		 * Screen size menu options
		 */
        private void actualToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetEmulationSize(1);
        }

        private void x2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetEmulationSize(2);
        }

        private void x3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetEmulationSize(3);
        }

        private void x4ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetEmulationSize(4);
        }

        private void x51280x960ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetEmulationSize(5);
        }

        private void x61536x1152ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetEmulationSize(6);
        }






        /// <summary>
        /// On form load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ZXForm_Load(object sender, EventArgs e)
        {
            TestStart();

			/*
            // initialise the display manager
            dm = new SubSystem.Display.DisplayManager(d3DControl);

            
            // disable default directx alt-enter (apparently doesnt work properly with winforms)
            using (var factory = d3DControl.SwapChain.GetParent<Factory>())
            {
                factory.SetWindowAssociation(this.Handle, WindowAssociationFlags.IgnoreAltEnter);
            }
			*/
            
			/*
            // setup custom fullscreen handler
            this.KeyDown += (o, e1) =>
            {
                if (e1.KeyCode == Keys.Enter)
                {
                    ToggleFullscreen();
                    //d3DControl.SwapChain.IsFullScreen = !d3DControl.SwapChain.IsFullScreen;
                }
                    
            };
			*/

            var t = new FakeTick();
        }

		/// <summary>
        /// On form resize event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ZXForm_ResizeEnd(object sender, EventArgs e)
        {
            /*
            WindowSize[0] = this.Width;
            WindowSize[1] = this.Height;
            SetRenderer((IRenderer)Activator.CreateInstance(typeof(D3DRenderer)));
            
			*/

            UnPause();
        }

        private void ZXForm_ResizeBegin(object sender, EventArgs e)
        {
            Pause();
        }


        

        /*
		/// <summary>
        /// maximise event
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0112) // WM_SYSCOMMAND
            {
                // Check your window state here
                if (m.WParam == new IntPtr(0xF030)) // Maximize event - SC_MAXIMIZE from Winuser.h
                {
                    //dm.WindowSizeChanged();
                }
            }
            base.WndProc(ref m);
        }
		*/

        private void ZXForm_Resize(object sender, EventArgs e)
        {
            //_rendererRunning = false;
            //_renderThread?.Abort();
            //WindowSize[0] = this.Width;
			//WindowSize[1] = this.Height;
            //SetRenderer((IRenderer)Activator.CreateInstance(typeof(D3DRenderer)));
        }

		/// <summary>
        /// Toggles fullscreen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toggleFullscreenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToggleFullscreen();
            ZXForm_ResizeEnd(this, EventArgs.Empty);
        }
    }
}
