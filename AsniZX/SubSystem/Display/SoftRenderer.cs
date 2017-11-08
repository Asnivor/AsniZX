using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AsniZX.SubSystem.Display
{
    public class SoftRenderer : Control, IRenderer
    {
        public RenderEngine Renderer => RenderEngine.Software;

        private FrameData framedatabuffer;

        private DisplayHandler _displayHandler;
        private ZXForm _form;

        private Bitmap _gameBitmap;
        private GCHandle _rawBitmap;

        public string RendererName => "Software";

        /// <summary>
        /// The init routine - called everytime a display size change or renderer change happens
        /// </summary>
        /// <param name="displayHandler"></param>
        public void Initialise(DisplayHandler dh)
        {
            if (dh == null) return;

            _displayHandler = dh;

            if (dh.Form == null) return;

            BackColor = Color.Gray;
            DoubleBuffered = true;
        }

        /// <summary>
        /// Stops rendering
        /// </summary>
        public void StopRendering()
        {
            if (_rawBitmap.IsAllocated) _rawBitmap.Free();
        }

        protected override void OnResize(EventArgs e)
        {
            Initialise(_displayHandler);
            base.OnResize(e);
        }

        public void Draw(FrameData fd)
        {
            _gameBitmap?.Dispose();
            if (_rawBitmap.IsAllocated) _rawBitmap.Free();

            // calculate padding (if screen isnt 4:3)
            /*
            int padL = 0;
            int padT = 0;
            int padB = 0;
            int padR = 0;

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

            // Create new bitmap with padding

    */
            _rawBitmap = GCHandle.Alloc(fd.Buffer, GCHandleType.Pinned);
            _gameBitmap = new Bitmap(fd.Width, fd.Height, fd.Width * 4, PixelFormat.Format32bppPArgb, _rawBitmap.AddrOfPinnedObject());

           Invalidate();
            //Update();
        }

        public void Draw()
        {
            Draw(framedatabuffer);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (_gameBitmap == null || _displayHandler.Form == null || !_displayHandler.Form.gameStarted) return;

            Graphics _renderTarget = e.Graphics;
            _renderTarget.CompositingMode = CompositingMode.SourceCopy;
            _renderTarget.InterpolationMode = _displayHandler.Form._filterMode == FilterMode.Linear ? InterpolationMode.Bilinear : InterpolationMode.NearestNeighbor;
            _renderTarget.DrawImage(_gameBitmap, 0, 0, Size.Width, Size.Height);
        }
    }
}
