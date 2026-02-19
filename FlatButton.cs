using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace VideoPlayer
{
    /// <summary>Botón personalizado con tema oscuro/azul.</summary>
    public class FlatButton : Button
    {
        private bool _hovered;
        private bool _pressed;

        public bool IsToggled { get; set; }
        public bool IsIconButton { get; set; } = false;

        public FlatButton()
        {
            SetStyle(ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer, true);
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            BackColor   = Theme.Surface2;
            ForeColor   = Theme.Accent;
            Font        = new Font("Segoe UI Symbol", 10f, FontStyle.Regular, GraphicsUnit.Point);
            Cursor      = Cursors.Hand;
            TabStop     = false;
        }

        protected override void OnMouseEnter(EventArgs e) { _hovered = true;  Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { _hovered = false; Invalidate(); base.OnMouseLeave(e); }
        protected override void OnMouseDown(MouseEventArgs e) { _pressed = true;  Invalidate(); base.OnMouseDown(e); }
        protected override void OnMouseUp(MouseEventArgs e)   { _pressed = false; Invalidate(); base.OnMouseUp(e); }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g   = e.Graphics;
            var rc  = ClientRectangle;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Fondo
            Color bg = _pressed  ? Theme.AccentDim   :
                       _hovered  ? Color.FromArgb(28, 50, 90) :
                       IsToggled ? Theme.AccentDim   :
                                   Theme.Surface2;

            using var bgBrush = new SolidBrush(bg);
            using var path = RoundRect(rc, 5);
            g.FillPath(bgBrush, path);

            // Borde sutil
            if (_hovered || IsToggled)
            {
                using var pen = new Pen(Theme.AccentDim, 1f);
                g.DrawPath(pen, path);
            }

            // Texto / ícono
            Color fg = _pressed ? Color.White :
                       IsToggled ? Theme.AccentHover :
                       _hovered  ? Color.White : Theme.Accent;

            TextRenderer.DrawText(g, Text, Font, rc, fg,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
        }

        private static GraphicsPath RoundRect(Rectangle rc, int r)
        {
            var path = new GraphicsPath();
            path.AddArc(rc.X, rc.Y, r * 2, r * 2, 180, 90);
            path.AddArc(rc.Right - r * 2, rc.Y, r * 2, r * 2, 270, 90);
            path.AddArc(rc.Right - r * 2, rc.Bottom - r * 2, r * 2, r * 2, 0, 90);
            path.AddArc(rc.X, rc.Bottom - r * 2, r * 2, r * 2, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
