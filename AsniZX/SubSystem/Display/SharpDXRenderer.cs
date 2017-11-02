using System;
using System.Windows.Forms;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using AlphaMode = SharpDX.Direct2D1.AlphaMode;
using Device = SharpDX.Direct3D11.Device;
using Factory = SharpDX.DXGI.Factory;
using SharpDX.Mathematics.Interop;
using Resource = SharpDX.Direct3D11.Resource;

namespace AsniZX.SubSystem.Display
{
    class SharpDXRenderer : Control, IRenderer, IDisposable
    {
        Device device;
        SwapChain swapChain;
        RenderTarget d2dRenderTarget;
        Bitmap gameBitmap;
        RawRectangleF clientArea;

        public int cWidth { get; set; }
        public int cHeight { get; set; }

        private FrameData framedatabuffer;

        private DisplayHandler _displayHandler;
        private ZXForm _form;
        private readonly Object _drawlock = new object();

        public RenderEngine Renderer => RenderEngine.SharpDX;

        public void Initialise(DisplayHandler dh)
        {
            _displayHandler = dh;

            cWidth = ClientSize.Width;
            cHeight = ClientSize.Height;

            lock (_drawlock)
            {
                if (dh.Form == null)
                    return;

                _form = dh.Form;
                ResizeRedraw = false;

                
                //clientArea = SetClientAreaSize(ClientSize.Width, ClientSize.Height);

                var desc = new SwapChainDescription
                {
                    BufferCount = 1,
                    ModeDescription = new ModeDescription(ClientSize.Width, ClientSize.Height, new Rational(30, 1),
                        Format.R8G8B8A8_UNorm),
                    IsWindowed = true,
                    OutputHandle = Handle,
                    SampleDescription = new SampleDescription(1, 0),
                    SwapEffect = SwapEffect.Discard,
                    Usage = Usage.RenderTargetOutput
                };

                Device.CreateWithSwapChain(DriverType.Hardware,
                    DeviceCreationFlags.BgraSupport,
                    new[] { SharpDX.Direct3D.FeatureLevel.Level_10_0 },
                    desc,
                    out device,
                    out swapChain);

                var d2dFactory = new SharpDX.Direct2D1.Factory();

                Factory factory = swapChain.GetParent<Factory>();
                factory.MakeWindowAssociation(Handle, WindowAssociationFlags.IgnoreAll);

                Texture2D backBuffer = Resource.FromSwapChain<Texture2D>(swapChain, 0);

                Surface surface = backBuffer.QueryInterface<Surface>();                

                d2dRenderTarget = new RenderTarget(d2dFactory, surface,
                    new RenderTargetProperties(new PixelFormat(Format.Unknown, AlphaMode.Premultiplied)));

                var bitmapProperties = new BitmapProperties(new PixelFormat(Format.B8G8R8A8_UNorm, AlphaMode.Ignore));

                gameBitmap = new Bitmap(d2dRenderTarget, new Size2(256, 192), bitmapProperties);
                if (framedatabuffer != null)
                    gameBitmap = new Bitmap(d2dRenderTarget, new Size2(framedatabuffer.Width, framedatabuffer.Height), bitmapProperties);

                // client area - maintain 4:3
                int padL = 0;
                int padT = 0;
                int padB = 0;
                int padR = 0;

                if (framedatabuffer != null)
                {
                    float scaleX = ((float)ClientSize.Width / (float)(framedatabuffer.Width));
                    float scaleY = ((float)ClientSize.Height / (float)(framedatabuffer.Height));

                    if (ClientSize.Height < ClientSize.Width)
                    {
                        // height is smaller and should always remain locked to the boundary
                        float aspectXScale = (ClientSize.Height * 4.0f) / (ClientSize.Width * 3.0f);
                        scaleX = (scaleX * aspectXScale);
                        int newWidth = (int)(ClientSize.Width * aspectXScale);
                        int newWidthLeftover = ClientSize.Width - newWidth;
                        padL = newWidthLeftover / 2;
                        padR = newWidthLeftover / 2;
                    }
                    else
                    {
                        // width is smaller and should always remain locked to the boundary
                        float aspectYScale = (ClientSize.Width * 3.0f) / (ClientSize.Height * 4.0f);
                        scaleY = (scaleY * aspectYScale);
                        int newHeight = (int)(ClientSize.Height * aspectYScale);
                        int newHeightLeftover = ClientSize.Height - newHeight;
                        padT = newHeightLeftover / 2;
                        padB = newHeightLeftover / 2;
                    }

                    if (scaleX < 1.0f)
                        scaleX = 1.0f;

                    if (scaleY < 1.0f)
                        scaleY = 1.0f;
                }
                clientArea = new RawRectangleF
                {
                    Left = padL,
                    Top = padT,
                    Bottom = ClientSize.Height - padB,
                    Right = ClientSize.Width - padR
                };

                /*
                clientArea = new RawRectangleF
                {
                    Left = 24,
                    Top = 48,
                    Bottom = ClientSize.Height - 48,
                    Right = ClientSize.Width - 24
                };
                */



                factory.Dispose();
                surface.Dispose();
                backBuffer.Dispose();
                _form.ready = true;
            }
        }

            private RawRectangleF SetClientAreaSize(int width, int height)
        {
            //_form.WindowSize[0] = width + 48;
            //_form.WindowSize[1] = height + 48;

            var ca = new RawRectangleF
            {
                Left = 0,
                Top = 0,
                Bottom = height,
                Right = width
            };


            return ca;
        }

        /// <summary>
        /// Accepts a framedata object and attempts to draw it to the screen
        /// </summary>
        /// <param name="fd"></param>
        public void Draw(FrameData fd)
        {
            if (fd == null)
                return;

            framedatabuffer = fd;

            lock (_drawlock)
            {
                if (_form == null || d2dRenderTarget == null || !_form.ready || d2dRenderTarget.IsDisposed) return;
                d2dRenderTarget.BeginDraw();
                d2dRenderTarget.Clear(Color.Black);

                if (_form.gameStarted)
                {
                    int stride = fd.Width * 4;
                    gameBitmap.CopyFromMemory(fd.Buffer, stride);

                    

                    d2dRenderTarget.DrawBitmap(gameBitmap, clientArea, 1f,
                        _form._filterMode == 
                        FilterMode.Linear
                            ? BitmapInterpolationMode.Linear
                            : BitmapInterpolationMode.NearestNeighbor);
                }

                d2dRenderTarget.EndDraw();
                swapChain.Present(0, PresentFlags.None);
            }
        }

        /// <summary>
        /// Draw routine
        /// </summary>
        /// 
        public void Draw()
        {
            Draw(framedatabuffer);
        }
            /*
        public void Draw()
        {
            lock (_drawlock)
            {
                if (_form == null || d2dRenderTarget == null || !_form.ready || d2dRenderTarget.IsDisposed) return;
                d2dRenderTarget.BeginDraw();
                d2dRenderTarget.Clear(Color.Red);

                if (_form.gameStarted)
                {
                    int stride = ZXForm.GameWidth * 4;
                    gameBitmap.CopyFromMemory(_form.rawBitmap, stride);

                    d2dRenderTarget.DrawBitmap(gameBitmap, clientArea, 1f,
                        _form._filterMode == FilterMode.Linear
                            ? BitmapInterpolationMode.Linear
                            : BitmapInterpolationMode.NearestNeighbor);
                }

                d2dRenderTarget.EndDraw();
                swapChain.Present(0, PresentFlags.None);
            }
        }
        */

        /// <summary>
        /// Stop routine
        /// </summary>
        public void StopRendering()
        {
            DisposeD3D();
        }

        public void ReceiveInput(FrameData input)
        {
            // check whether anything has changed

        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Draw();
            base.OnPaint(e);
        }

        protected override void OnResize(EventArgs e)
        {
            try
            {
                DisposeD3D();
                Initialise(_displayHandler);
                base.OnResize(e);
            }
            catch
            {
            }
        }

        private void DisposeD3D()
        {
            lock (_drawlock)
            {
                if (_form != null && _form.ready)
                {
                    _form.ready = false;
                    d2dRenderTarget.Dispose();
                    swapChain.Dispose();
                    device.Dispose();
                    gameBitmap.Dispose();
                }
            }
        }
    }
}
