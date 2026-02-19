using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace VideoPlayer
{
    /// <summary>Control de volumen compacto con diseño minimalista.</summary>
    public class VolumeBar : Control
    {
        private int  _value = 100;
        private bool _dragging;
        private bool _hovered;

        public event EventHandler? VolumeChanged;

        public int Value
        {
            get => _value;
            set { _value = Math.Max(0, Math.Min(100, value)); Invalidate(); VolumeChanged?.Invoke(this, EventArgs.Empty); }
        }

        public VolumeBar()
        {
            SetStyle(ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer, true);
            Height = 22;
            Width  = 90;
            Cursor = Cursors.Hand;
            BackColor = Theme.Background;
        }

        protected override void OnMouseEnter(EventArgs e) { _hovered = true;  Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { _hovered = false; Invalidate(); base.OnMouseLeave(e); }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            _dragging = true; Capture = true;
            UpdateFromMouse(e.X); base.OnMouseDown(e);
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_dragging) UpdateFromMouse(e.X); base.OnMouseMove(e);
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            _dragging = false; Capture = false;
            UpdateFromMouse(e.X); Invalidate(); base.OnMouseUp(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            Value += e.Delta > 0 ? 5 : -5;
            base.OnMouseWheel(e);
        }

        private void UpdateFromMouse(int x)
        {
            int left  = 6;
            int right = Width - 6;
            int w     = right - left;
            if (w <= 0) return;
            float ratio = (float)(x - left) / w;
            Value = (int)(Math.Max(0f, Math.Min(1f, ratio)) * 100);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g  = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var rc = ClientRectangle;

            int left   = 6;
            int right  = rc.Width - 6;
            int w      = right - left;
            int cy     = rc.Height / 2;
            int th     = _hovered ? 4 : 3;

            float pct  = _value / 100f;
            int   fillX = left + (int)(pct * w);

            // Track
            var trackRc = new RectangleF(left, cy - th / 2f, w, th);
            using var bgBrush = new SolidBrush(Theme.Surface2);
            g.FillRectangle(bgBrush, trackRc);

            // Fill
            if (fillX > left)
            {
                var fillRc = new RectangleF(left, cy - th / 2f, fillX - left, th);
                using var fillBrush = new SolidBrush(Theme.Accent);
                g.FillRectangle(fillBrush, fillRc);
            }

            // Thumb pequeño
            int tr = _hovered ? 5 : 4;
            var thumbRc = new RectangleF(fillX - tr, cy - tr, tr * 2, tr * 2);
            using var thumbBrush = new SolidBrush(_hovered ? Theme.AccentHover : Theme.Accent);
            g.FillEllipse(thumbBrush, thumbRc);
        }
    }
}
