using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AsniZX
{
    public class FakeTick
    {
        Timer t = new Timer();

        public FakeTick()
        {
            t.Interval = 3000;
            t.Tick += new EventHandler(timer_Tick);
            t.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            //ZXForm.dm.DoPaint();
            //ZXForm.dm.Control.SwapChain.IsFullScreen = ZXForm.dm.Control.SwapChain.IsFullScreen;
        }

    }
}
