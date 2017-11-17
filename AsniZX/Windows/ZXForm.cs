using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

using AsniZX.SubSystem.Display;
using AsniZX.SubSystem.Input;
using AsniZX.SubSystem.Sound;
using AsniZX.Common;

namespace AsniZX
{
    public partial class ZXForm : Form
    {
        public static ZXForm MainForm { get; private set; }

        public DisplayHandler displayHandler { get; set; }

        public Emulation.EmulationMachine EmuMachine { get; set; }

        public bool IsFullScreen { get; set; }


        //public Sound Sound { get; set; }



        /// <summary>
        /// Current frames-per-second calculation
        /// </summary>
        public int FPS { get; set; }

        /*
        public static SubSystem.Display.DisplayManager dm;
        */
        public bool AppFullscreen { get; set; }
        public Point WindowPosition { get; set; }
        public int[] WindowSize { get; set; }

        public bool ready;
        public IRenderer _renderer;

        public FilterMode _filterMode = FilterMode.NearestNeighbor;


        private int[] speeds = { 1, 2, 4, 8, 16 };
        private string[] sizes = { "1x", "2x", "4x", "8x" };
        
        public bool gameStarted;

        public bool isPaused { get; set; }

        public ZXForm()
        {
            InitializeComponent();

            MainForm = this;

            Global.Config = new Config();

            displayHandler = new DisplayHandler();

            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = false;
                Thread.CurrentThread.Priority = ThreadPriority.Highest;
                
            }).Start();

            //Sound = new Sound(this);
            //Sound.StartSound();



            this.Width = displayHandler.inputWidth * displayHandler.magnification;
            this.Height = (displayHandler.inputHeight + statusStrip1.Height + MainMenuStrip.Height) * displayHandler.magnification;

            this.MinimumSize = new Size(displayHandler.inputWidth, displayHandler.inputHeight + statusStrip1.Height + MainMenuStrip.Height);

            WindowSize = new int[2];
            WindowSize[0] = this.Width;
            WindowSize[1] = this.Height;

            isPaused = false;
            
            togglePauseToolStripMenuItem.Text = "Pause (F11)";

            gameStarted = true;

            IsFullScreen = false;

            KeyboardHandler.Initialize(this);
            SoundHandler.Init();


            //_keyboardHandler = new KeyboardHandler();
            

            //SetRenderer((IRenderer)Activator.CreateInstance(typeof(D3DRenderer)));    
        }

        /// <summary>
        /// sets actual FPS
        /// </summary>
        /// <param name="val"></param>
        public void SetFPS(int val)
        {
            toolStripStatusLabel1.Text = "Actual FPS: " + val;
        }

        public void SetVirtualFPS(int val)
        {
            toolStripStatusLabel2.Text = "Virtual Speed: " + val;
        }

        /*
        private void SetRenderer(IRenderer renderer)
        {
            if (_renderer == renderer)
                return;

            if (_renderer != null)
            {
                var oldCtrl = (Control)renderer;
                _renderer.StopRendering();
                Controls.Remove((Control)_renderer);
            }

            _renderer = renderer;
            var ctrl = (Control)_renderer;
            ctrl.Dock = DockStyle.Fill;
            ctrl.TabStop = false;
            Controls.Add(ctrl);
            _renderer.Initialise(this);
        }
        */

        public void Pause()
        {
            togglePauseToolStripMenuItem.Text = "UnPause (F11)";
        }

        public void UnPause()
        {
            togglePauseToolStripMenuItem.Text = "Pause (F11)";
        }

        public void TogglePause()
        {
            if (EmuMachine.IsSuspended == true)
            {
                EmuMachine.UnPause();
            }
                
            else
            {
                EmuMachine.Pause();
            }
             
        }

        private void TestStart()
        {
            /*
            Speccy = new ZXSpectrum.Spectrum(SpecModel._48k);
            Speccy.StartEmulation();
            */


            EmuMachine = new Emulation.EmulationMachine();
            EmuMachine.Start();

            /*
            emu = new Emulator();

            _renderThread = new Thread(() =>
            {
                gameStarted = true;

                Stopwatch s = new Stopwatch();
                Stopwatch s0 = new Stopwatch();

                while (_rendererRunning)
                {
                    if (suspended)
                    {
                        Thread.Sleep(100);
                        continue;
                    }

                    for (int i = 0; i < 60 && !suspended; i++)
                    {
                        s0.Restart();
                        emu.ProcessFrame();
                        //rawBitmap = emu.RawBitmap;
                        //Invoke((MethodInvoker)_renderer.Draw);
                        s0.Stop();
                        Thread.Sleep(Math.Max((int)(980 / 60.0 - s0.ElapsedMilliseconds), 0) / activeSpeed);
                    }
                }
            });
            _renderThread.Start();
            */
        }

        

       
        /// <summary>
        /// Toggles fullscreen mode on/off
        /// </summary>
        /// <param name="fs"></param>
        public void ToggleFullscreen()
        {
            if (AppFullscreen)
            {
                // we are currently fullscreen
                GoWindowed();
                IsFullScreen = false;
                return;
            }
            if (!AppFullscreen)
            {
                // we are currently windowed
                GoFullScreen();
                IsFullScreen = true;
                return;
            }
        }

        /// <summary>
        /// Goes fullscreen if app is currently in windowed mode
        /// </summary>
        public void GoFullScreen()
        {
            if (AppFullscreen)
                return;

            AppFullscreen = true;

            MainMenuStrip.Visible = false;
            statusStrip1.Visible = false;
            this.SuspendLayout();
            this.FormBorderStyle = FormBorderStyle.None;
            WindowPosition = this.Location;
            WindowSize[0] = this.Width;
            WindowSize[1] = this.Height;

            this.Location = new Point(0, 0);
            this.WindowState = FormWindowState.Maximized;

            

            this.ResumeLayout();
        }

        /// <summary>
        /// Goes windowed if app is currently in fullscreen mode
        /// </summary>
        public void GoWindowed()
        {
            if (!AppFullscreen)
                return;

            AppFullscreen = false;

            MainMenuStrip.Visible = true;
            statusStrip1.Visible = true;
            this.SuspendLayout();
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.WindowState = FormWindowState.Normal;

            this.Location = WindowPosition;
            this.Width = WindowSize[0];
            this.Height = WindowSize[1] + statusStrip1.Height + MainMenuStrip.Height;

            this.ResumeLayout();
        }

        /// <summary>
        /// Sets the emulation to a pre-defined scaling factor
        /// </summary>
        /// <param name="magnification"></param>
        public void SetEmulationSize(int magnification)
        {
            displayHandler.magnification = magnification;
            this.Width = displayHandler.inputWidth * magnification;
            this.Height = (displayHandler.inputHeight * magnification) + statusStrip1.Height + MainMenuStrip.Height;
            displayHandler.Init();
        }

        private void toolStripStatusLabel2_Click(object sender, EventArgs e)
        {

        }

        
    }
}
