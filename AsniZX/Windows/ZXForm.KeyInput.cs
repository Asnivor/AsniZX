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

        private bool ProcessKeyDown(Keys keyCode)
        {
            switch (keyCode)
            {
                case Keys.F12:
                    ToggleFullscreen();
                    ZXForm_ResizeEnd(this, EventArgs.Empty);
                    return true;

                case Keys.F11:
                    TogglePause();
                    return true;

                case Keys.Up:
                    {
                        // act on up arrow
                        return true;
                    }
                case Keys.Down:
                    {
                        // act on down arrow
                        return true;
                    }
                case Keys.Left:
                    {
                        // act on left arrow
                        return true;
                    }
                case Keys.Right:
                    {
                        // act on right arrow
                        return true;
                    }
            }
            return false;
        }
    }
}
