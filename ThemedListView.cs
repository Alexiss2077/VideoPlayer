// ════════════════════════════════════════════════════════════════════════════
//  ThemedListView.cs
//  ListView con header y filas pintadas con el tema oscuro.

// ════════════════════════════════════════════════════════════════════════════

using System;
using System.Drawing;
using System.Windows.Forms;

namespace VideoPlayer
{
    internal sealed class ThemedListView : ListView
    {
        // ── Colores ───────────────────────────────────────────────────────
        private static readonly Color RowEven = Color.FromArgb(10, 16, 28);
        private static readonly Color RowOdd = Color.FromArgb(13, 20, 36);
        private static readonly Color RowSelected = Color.FromArgb(16, 32, 60);   // azul neutro, no satura
        private static readonly Color RowHover = Color.FromArgb(15, 26, 48);   // hover: apenas visible
        private static readonly Color RowCurrent = Theme.HighlightRow;           // en reproducción: más saturado

        private int _hoveredIndex = -1;

        // ════════════════════════════════════════════════════════════════
        public ThemedListView()
        {
            this.OwnerDraw = true;
            this.DoubleBuffered = true;
            this.FullRowSelect = true;
            this.GridLines = false;
            this.BorderStyle = BorderStyle.None;
            this.BackColor = RowEven;
            this.ForeColor = Theme.TextPrimary;
            this.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            this.HideSelection = false;
            this.MultiSelect = false;

            this.DrawItem += OnDrawItem;
            this.DrawSubItem += OnDrawSubItem;
            this.MouseMove += OnMouseMove2;
            this.MouseLeave += OnMouseLeave2;
        }

        // ════════════════════════════════════════════════════════════════
        //  Filas
        // ════════════════════════════════════════════════════════════════
        private void OnDrawItem(object? sender, DrawListViewItemEventArgs e)
        {
            var g = e.Graphics;
            var rc = e.Bounds;

            bool sel = (e.State & ListViewItemStates.Selected) != 0;
            bool hovered = e.ItemIndex == _hoveredIndex;
            bool current = e.Item.BackColor == Theme.HighlightRow;

            Color bg = current ? RowCurrent :   // reproduciendo → más saturado
                       sel ? RowSelected :   // seleccionado  → azul suave
                       hovered ? RowHover :   // hover         → apenas visible
                       e.ItemIndex % 2 == 0 ? RowEven : RowOdd;

            using var bgBrush = new SolidBrush(bg);
            g.FillRectangle(bgBrush, rc);

            // Barra izquierda: azul brillante para "reproduciendo", gris-azul para seleccionado
            if (current)
            {
                using var accent = new SolidBrush(Theme.Accent);
                g.FillRectangle(accent, new Rectangle(rc.Left, rc.Top, 3, rc.Height));
            }
            else if (sel)
            {
                using var selBar = new SolidBrush(Theme.AccentDim);
                g.FillRectangle(selBar, new Rectangle(rc.Left, rc.Top, 3, rc.Height));
            }

            // Línea divisoria inferior sutil
            using var div = new Pen(Color.FromArgb(18, 255, 255, 255), 1f);
            g.DrawLine(div, rc.Left + 3, rc.Bottom - 1, rc.Right, rc.Bottom - 1);
        }

        private void OnDrawSubItem(object? sender, DrawListViewSubItemEventArgs e)
        {
            if (e.Item == null || e.SubItem == null) return;

            bool sel = (e.ItemState & ListViewItemStates.Selected) != 0;
            bool current = e.Item.BackColor == Theme.HighlightRow;

            // current  → colores brillantes (azul acento / blanco)
            // selected → colores intermedios (no saturados)
            // normal   → colores base del tema
            Color fg = e.ColumnIndex == 0
                ? (current ? Theme.AccentHover
                   : sel ? Theme.AccentDim
                           : Theme.TextDim)
                : e.ColumnIndex == 1
                ? (current ? Color.White
                   : sel ? Color.FromArgb(200, 215, 235)   // blanco frío suave
                           : Theme.TextPrimary)
                : (current ? Theme.Accent
                   : sel ? Color.FromArgb(100, 150, 200)   // azul apagado
                           : Theme.TextSecondary);

            string text = e.SubItem.Text;
            if (e.ColumnIndex == 0 && current) text = "▶";

            var align = (e.ColumnIndex == 0 || e.ColumnIndex >= 2)
                ? TextFormatFlags.HorizontalCenter : TextFormatFlags.Left;

            TextRenderer.DrawText(e.Graphics, text, this.Font,
                new Rectangle(e.Bounds.Left + 4, e.Bounds.Top,
                              e.Bounds.Width - 6, e.Bounds.Height),
                fg,
                TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis |
                align | TextFormatFlags.NoPadding);
        }

        // ════════════════════════════════════════════════════════════════
        //  Hover
        // ════════════════════════════════════════════════════════════════
        private void OnMouseMove2(object? sender, MouseEventArgs e)
        {
            int idx = HitTest(e.Location).Item?.Index ?? -1;
            if (idx != _hoveredIndex) { _hoveredIndex = idx; Invalidate(); }
        }

        private void OnMouseLeave2(object? sender, EventArgs e)
        {
            _hoveredIndex = -1; Invalidate();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            e.Graphics.Clear(RowEven);
        }
    }
}