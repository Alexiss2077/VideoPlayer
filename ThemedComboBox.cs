// ════════════════════════════════════════════════════════════════════════════
//  ThemedComboBox.cs
//  ComboBox con fondo, texto, borde y flecha pintados con el tema oscuro.
//  Usa DrawMode.OwnerDrawFixed para controlar cada píxel.
// ════════════════════════════════════════════════════════════════════════════

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace VideoPlayer
{
    internal sealed class ThemedComboBox : ComboBox
    {
        // ── Colores ───────────────────────────────────────────────────────
        // Fondo del control
        private static readonly Color BgNormal = Color.FromArgb(16, 26, 46);
        private static readonly Color BgHover = Color.FromArgb(20, 34, 60);
        private static readonly Color BgDropdown = Color.FromArgb(12, 20, 36);
        private static readonly Color BorderColor = Color.FromArgb(32, 60, 100);
        private static readonly Color BorderHover = Theme.AccentDim;
        private static readonly Color TextColor = Color.FromArgb(180, 205, 230); // blanco frío, no saturado
        private static readonly Color ArrowColor = Color.FromArgb(80, 140, 200);  // azul apagado
        private static readonly Color ItemHover = Color.FromArgb(22, 40, 72);

        private bool _isHovered = false;

        // ════════════════════════════════════════════════════════════════
        public ThemedComboBox()
        {
            this.DrawMode = DrawMode.OwnerDrawFixed;
            this.DropDownStyle = ComboBoxStyle.DropDownList;
            this.FlatStyle = FlatStyle.Flat;
            this.BackColor = BgNormal;
            this.ForeColor = TextColor;
            this.ItemHeight = 20;

            this.MouseEnter += (_, _) => { _isHovered = true; Invalidate(); };
            this.MouseLeave += (_, _) => { _isHovered = false; Invalidate(); };
        }

        // ── Dibujar el control cerrado ────────────────────────────────────
        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var rc = ClientRectangle;

            // Fondo
            var bg = _isHovered ? BgHover : BgNormal;
            using var bgBrush = new SolidBrush(bg);
            g.FillRectangle(bgBrush, rc);

            // Borde redondeado suave
            var border = _isHovered ? BorderHover : BorderColor;
            using var borderPen = new Pen(border, 1f);
            g.DrawRectangle(borderPen,
                new Rectangle(rc.Left, rc.Top, rc.Width - 1, rc.Height - 1));

            // Zona de la flecha
            int arrowW = 18;
            var arrowZone = new Rectangle(rc.Right - arrowW, rc.Top + 1,
                                          arrowW - 1, rc.Height - 2);
            using var arrowBg = new SolidBrush(Color.FromArgb(_isHovered ? 28 : 20, 50, 88));
            g.FillRectangle(arrowBg, arrowZone);

            // Línea separadora antes de la flecha
            using var sepPen = new Pen(border, 1f);
            g.DrawLine(sepPen, arrowZone.Left, rc.Top + 3, arrowZone.Left, rc.Bottom - 3);

            // Flecha hacia abajo (triángulo)
            int ax = arrowZone.Left + arrowZone.Width / 2;
            int ay = rc.Height / 2;
            var arrow = new Point[]
            {
                new Point(ax - 4, ay - 2),
                new Point(ax + 4, ay - 2),
                new Point(ax,     ay + 3)
            };
            using var arrowBrush = new SolidBrush(ArrowColor);
            g.FillPolygon(arrowBrush, arrow);

            // Texto del ítem seleccionado
            if (SelectedIndex >= 0)
            {
                string text = Items[SelectedIndex]?.ToString() ?? "";
                var textRc = new Rectangle(rc.Left + 6, rc.Top,
                                            rc.Width - arrowW - 8, rc.Height);
                TextRenderer.DrawText(g, text, this.Font, textRc, TextColor,
                    TextFormatFlags.VerticalCenter |
                    TextFormatFlags.Left |
                    TextFormatFlags.NoPadding);
            }
        }

        // ── Dibujar cada ítem del desplegable ─────────────────────────────
        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            var g = e.Graphics;
            var rc = e.Bounds;

            bool selected = (e.State & DrawItemState.Selected) != 0;
            bool focus = (e.State & DrawItemState.Focus) != 0;

            // Fondo del ítem
            var bg = selected ? ItemHover : BgDropdown;
            using var bgBrush = new SolidBrush(bg);
            g.FillRectangle(bgBrush, rc);

            // Línea azul izquierda en el ítem seleccionado
            if (selected)
            {
                using var bar = new SolidBrush(Theme.AccentDim);
                g.FillRectangle(bar, new Rectangle(rc.Left, rc.Top, 2, rc.Height));
            }

            // Texto
            string text = Items[e.Index]?.ToString() ?? "";
            var fg = selected ? Color.FromArgb(200, 220, 245) : TextColor;
            TextRenderer.DrawText(g, text, this.Font,
                new Rectangle(rc.Left + 8, rc.Top, rc.Width - 10, rc.Height),
                fg,
                TextFormatFlags.VerticalCenter |
                TextFormatFlags.Left |
                TextFormatFlags.NoPadding);
        }

        // Necesario para que OnPaint se llame correctamente en un ComboBox
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            // WM_PAINT = 0x000F
            if (m.Msg == 0x000F) OnPaint(new PaintEventArgs(
                System.Drawing.Graphics.FromHwnd(m.HWnd), ClientRectangle));
        }
    }
}