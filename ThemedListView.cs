// ════════════════════════════════════════════════════════════════════════════
//  ThemedListView.cs
//  ListView con cabecera y filas pintadas con el tema oscuro.
//  Incluye HeaderWindow que intercepta WM_ERASEBKGND para eliminar el
//  cuadro blanco residual a la derecha de las columnas.
// ════════════════════════════════════════════════════════════════════════════

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace VideoPlayer
{
    internal sealed class ThemedListView : ListView
    {
        // ── Colores ───────────────────────────────────────────────────────
        private static readonly Color RowEven = Color.FromArgb(10, 16, 28);
        private static readonly Color RowOdd = Color.FromArgb(13, 20, 36);
        private static readonly Color RowSelected = Theme.HighlightRow;
        private static readonly Color RowHover = Color.FromArgb(18, 30, 55);
        private static readonly Color HeaderBg = Color.FromArgb(14, 22, 40);
        private static readonly Color ColSep = Color.FromArgb(22, 38, 68);

        private int _hoveredIndex = -1;

        // Subclase nativa del header para interceptar WM_ERASEBKGND
        private HeaderWindow? _headerWindow;

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

            this.DrawColumnHeader += OnDrawColumnHeader;
            this.DrawItem += OnDrawItem;
            this.DrawSubItem += OnDrawSubItem;
            this.MouseMove += OnMouseMove2;
            this.MouseLeave += OnMouseLeave2;
        }

        // ── Adjuntar la subclase nativa al header una vez creado el handle ─
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            // LVM_GETHEADER = 0x101F  → obtiene el HWND del control header
            IntPtr hHeader = SendMessage(this.Handle, 0x101F, IntPtr.Zero, IntPtr.Zero);
            if (hHeader != IntPtr.Zero)
            {
                _headerWindow = new HeaderWindow(hHeader, HeaderBg);
            }
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            _headerWindow?.ReleaseHandle();
            _headerWindow = null;
            base.OnHandleDestroyed(e);
        }

        // ════════════════════════════════════════════════════════════════
        //  Dibujo del header
        // ════════════════════════════════════════════════════════════════
        private void OnDrawColumnHeader(object? sender, DrawListViewColumnHeaderEventArgs e)
        {
            var g = e.Graphics;
            var rc = e.Bounds;

            // Fondo
            using var bg = new SolidBrush(HeaderBg);
            g.FillRectangle(bg, rc);

            // Línea azul inferior
            using var borderPen = new Pen(Theme.AccentDim, 1f);
            g.DrawLine(borderPen, rc.Left, rc.Bottom - 1, rc.Right, rc.Bottom - 1);

            // Separador vertical
            if (e.ColumnIndex > 0)
            {
                using var sep = new Pen(ColSep, 1f);
                g.DrawLine(sep, rc.Left, rc.Top + 4, rc.Left, rc.Bottom - 4);
            }

            // Sin texto en columnas ocultas
            if (rc.Width <= 4) return;

            var align = e.Header?.TextAlign == HorizontalAlignment.Center
                ? TextFormatFlags.HorizontalCenter : TextFormatFlags.Left;

            var textRc = new Rectangle(rc.Left + 8, rc.Top, rc.Width - 12, rc.Height);
            TextRenderer.DrawText(g, e.Header?.Text ?? "",
                Theme.FontTitle, textRc, Theme.Accent,
                TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis |
                align | TextFormatFlags.NoPadding);
        }

        // ════════════════════════════════════════════════════════════════
        //  Dibujo de filas
        // ════════════════════════════════════════════════════════════════
        private void OnDrawItem(object? sender, DrawListViewItemEventArgs e)
        {
            var g = e.Graphics;
            var rc = e.Bounds;

            bool sel = (e.State & ListViewItemStates.Selected) != 0;
            bool hovered = e.ItemIndex == _hoveredIndex;
            bool current = e.Item.BackColor == Theme.HighlightRow;

            Color bg = sel || current ? RowSelected :
                       hovered ? RowHover :
                       e.ItemIndex % 2 == 0 ? RowEven : RowOdd;

            using var bgBrush = new SolidBrush(bg);
            g.FillRectangle(bgBrush, rc);

            // Barra azul izquierda en ítem activo / seleccionado
            if (sel || current)
            {
                using var accent = new SolidBrush(Theme.Accent);
                g.FillRectangle(accent, new Rectangle(rc.Left, rc.Top, 3, rc.Height));
            }

            // Línea divisoria sutil
            using var div = new Pen(Color.FromArgb(18, 255, 255, 255), 1f);
            g.DrawLine(div, rc.Left + 3, rc.Bottom - 1, rc.Right, rc.Bottom - 1);
        }

        private void OnDrawSubItem(object? sender, DrawListViewSubItemEventArgs e)
        {
            if (e.Item == null || e.SubItem == null) return;

            bool sel = (e.ItemState & ListViewItemStates.Selected) != 0;
            bool current = e.Item.BackColor == Theme.HighlightRow;

            Color fg = e.ColumnIndex == 0 ? (sel || current ? Theme.AccentHover : Theme.TextDim) :
                       e.ColumnIndex == 1 ? (sel || current ? Color.White : Theme.TextPrimary) :
                                            (sel || current ? Theme.Accent : Theme.TextSecondary);

            string text = e.SubItem.Text;
            if (e.ColumnIndex == 0 && current) text = "▶";

            var align = (e.ColumnIndex == 0 || e.ColumnIndex >= 2)
                ? TextFormatFlags.HorizontalCenter : TextFormatFlags.Left;

            var rc = new Rectangle(e.Bounds.Left + 4, e.Bounds.Top,
                                   e.Bounds.Width - 6, e.Bounds.Height);

            TextRenderer.DrawText(e.Graphics, text, this.Font, rc, fg,
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

        // ════════════════════════════════════════════════════════════════
        //  Subclase nativa del Header
        //  Intercepta WM_ERASEBKGND para pintar el fondo oscuro en el
        //  área vacía que queda a la derecha de las columnas.
        // ════════════════════════════════════════════════════════════════
        private sealed class HeaderWindow : NativeWindow
        {
            private const int WM_ERASEBKGND = 0x0014;
            private const int WM_PAINT = 0x000F;
            private readonly Color _bg;

            public HeaderWindow(IntPtr handle, Color bg)
            {
                _bg = bg;
                AssignHandle(handle);
            }

            protected override void WndProc(ref Message m)
            {
                if (m.Msg == WM_ERASEBKGND && m.WParam != IntPtr.Zero)
                {
                    // Rellenar el fondo completo del header con nuestro color
                    using var g = Graphics.FromHdc(m.WParam);
                    using var hdc = new System.Drawing.BufferedGraphicsContext()
                                        .Allocate(g, new Rectangle(0, 0, 9999, 9999));

                    RECT rc;
                    GetClientRect(m.HWnd, out rc);
                    using var brush = new SolidBrush(_bg);
                    g.FillRectangle(brush,
                        new Rectangle(rc.Left, rc.Top,
                                      rc.Right - rc.Left,
                                      rc.Bottom - rc.Top));

                    m.Result = (IntPtr)1;   // indicar que se procesó
                    return;
                }
                base.WndProc(ref m);
            }

            [DllImport("user32.dll")]
            private static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

            [StructLayout(LayoutKind.Sequential)]
            private struct RECT
            {
                public int Left, Top, Right, Bottom;
            }
        }

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg,
                                                 IntPtr wParam, IntPtr lParam);
    }
}