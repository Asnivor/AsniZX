using AsniZX.Emulation.FileFormats.Snapshot;
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
            EmuMachine?.Stop();
            Application.Exit();
        }

        private void ZXForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Speccy.EmulationThread?.Abort();
            EmuMachine?.Stop();
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


        private void togglePauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TogglePause();
        }

        /// <summary>
        /// On form load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ZXForm_Load(object sender, EventArgs e)
        {
            TestStart();
        }

		/// <summary>
        /// On form resize event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ZXForm_ResizeEnd(object sender, EventArgs e)
        {
            EmuMachine.UnPause();
        }

        private void ZXForm_ResizeBegin(object sender, EventArgs e)
        {
            EmuMachine.Pause();
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

        /// <summary>
        /// hard reset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void terminateEmulationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EmuMachine.Spectrum.Reset();
        }

        /// <summary>
        /// soft reset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void softResetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EmuMachine.Spectrum.Reset();
        }

        /// <summary>
        /// loads various snapshot files
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadSnapshotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string fileName = "";
            OpenFileDialog o = new OpenFileDialog();

            o.Filter = "Snapshot Files|*.sna|All files (*.*)|*.*";

            DialogResult dr = o.ShowDialog();

            if (dr == DialogResult.OK)
            {
                fileName = o.FileName;
                SnapshotHandler.LoadSnapshot(fileName);
            }
            else
            {
                return;
            }
        }
    }
}
