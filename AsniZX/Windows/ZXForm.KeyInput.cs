using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AsniZX
{
    public partial class ZXForm : Form
    {
        private void ZXForm_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = ProcessKeyDown(e.KeyCode);
        }

        private void ZXForm_KeyUp(object sender, KeyEventArgs e)
        {
            e.Handled = ProcessKeyUp(e.KeyCode);
        }

        private bool ProcessKeyDown(Keys keyCode)
        {
            // Keydown events for non-emulator (i.e. UI) keypresses
            switch (keyCode)
            {
                case Keys.F12:
                    ToggleFullscreen();
                    ZXForm_ResizeEnd(this, EventArgs.Empty);
                    return true;
                case Keys.F11:
                    TogglePause();
                    return true;               
            }




            return false;
        }

        private bool ProcessKeyUp(Keys keyCode)
        {
            // Keyup events for non-emulator (i.e. UI) keypresses
            switch (keyCode)
            {
                case Keys.F12:
                case Keys.F11:
                    return true;
            }




            return false;
        }
    }
}
