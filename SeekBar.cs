using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace VideoPlayer
{
    /// <summary>Barra de progreso/seek personalizada con diseño oscuro/azul.</summary>
    public class SeekBar : Control
    {
        private int   _value;
        private int   _maximum = 1000;
        private bool  _dragging;
        private bool  _hovered;

        public event EventHandler? SeekRequested;
        public event EventHandler? SeekStarted;
        public event EventHandler? SeekEnded;

        public int Value
        {
            get => _value;
            set
            {
                _value = Math.Max(0, Math.Min(_maximum, value));
                if (!_dragging) Invalidate();
            }
        }

        public int Maximum
        {
            get => _maximum;
            set { _maximum = Math.Max(1, value); Invalidate(); }
        }

        public bool IsDragging => _dragging;

        public SeekBar()
        {
            SetStyle(ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer, true);
            Height  = 22;
            Cursor  = Cursors.Hand;
            BackColor = Theme.Background;
        }

        protected override void OnMouseEnter(EventArgs e) { _hovered = true;  Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { _hovered = false; Invalidate(); base.OnMouseLeave(e); }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            _dragging = true;
            SeekStarted?.Invoke(this, EventArgs.Empty);
            UpdateFromMouse(e.X);
            Capture = true;
            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_dragging) UpdateFromMouse(e.X);
            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (_dragging && e.Button == MouseButtons.Left)
            {
                _dragging = false;
                UpdateFromMouse(e.X);
                SeekEnded?.Invoke(this, EventArgs.Empty);
                Capture = false;
                Invalidate();
            }
            base.OnMouseUp(e);
        }

        private void UpdateFromMouse(int x)
        {
            int trackLeft  = 8;
            int trackRight = Width - 8;
            int trackWidth = trackRight - trackLeft;
            if (trackWidth <= 0) return;

            float ratio = (float)(x - trackLeft) / trackWidth;
            ratio = Math.Max(0f, Math.Min(1f, ratio));
            _value = (int)(ratio * _maximum);
            SeekRequested?.Invoke(this, EventArgs.Empty);
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g   = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var rc  = ClientRectangle;

            int trackLeft   = 8;
            int trackRight  = rc.Width - 8;
            int trackWidth  = trackRight - trackLeft;
            int trackY      = rc.Height / 2;
            int trackH      = _hovered || _dragging ? 5 : 3;
            int thumbR      = _hovered || _dragging ? 8 : 6;

            float pct = _maximum > 0 ? (float)_value / _maximum : 0f;
            int   fillX = trackLeft + (int)(pct * trackWidth);

            // Track fondo
            var trackRc = new RectangleF(trackLeft, trackY - trackH / 2f, trackWidth, trackH);
            using var trackPath = RoundRect(trackRc, trackH / 2);
            using var bgBrush = new SolidBrush(Theme.Surface2);
            g.FillPath(bgBrush, trackPath);

            // Track fill
            if (fillX > trackLeft)
            {
                var fillRc = new RectangleF(trackLeft, trackY - trackH / 2f, fillX - trackLeft, trackH);
                using var fillPath = RoundRect(fillRc, trackH / 2);
                using var fillBrush = new LinearGradientBrush(
                    new PointF(trackLeft, 0), new PointF(fillX, 0),
                    Theme.AccentDim, Theme.Accent);
                g.FillPath(fillBrush, fillPath);
            }

            // Thumb
            var thumbCenter = new PointF(fillX, trackY);
            var thumbRc = new RectangleF(thumbCenter.X - thumbR, thumbCenter.Y - thumbR, thumbR * 2, thumbR * 2);
            using var thumbBrush = new SolidBrush(_dragging ? Theme.AccentHover : Theme.Accent);
            g.FillEllipse(thumbBrush, thumbRc);

            // Thumb borde
            using var thumbPen = new Pen(Color.FromArgb(40, 255, 255, 255), 1f);
            g.DrawEllipse(thumbPen, thumbRc);
        }

        private static GraphicsPath RoundRect(RectangleF rc, float r)
        {
            var path = new GraphicsPath();
            if (rc.Width <= 0) return path;
            r = Math.Min(r, Math.Min(rc.Width / 2f, rc.Height / 2f));
            path.AddArc(rc.X, rc.Y, r * 2, r * 2, 180, 90);
            path.AddArc(rc.Right - r * 2, rc.Y, r * 2, r * 2, 270, 90);
            path.AddArc(rc.Right - r * 2, rc.Bottom - r * 2, r * 2, r * 2, 0, 90);
            path.AddArc(rc.X, rc.Bottom - r * 2, r * 2, r * 2, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
