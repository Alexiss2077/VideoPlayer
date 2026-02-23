// ════════════════════════════════════════════════════════════════════════════
//  PlaylistColumnHeader.cs
//  Panel que dibuja la cabecera de columnas de la playlist.
// ════════════════════════════════════════════════════════════════════════════

using System.Drawing;
using System.Windows.Forms;

namespace VideoPlayer
{
    internal sealed class PlaylistColumnHeader : Control
    {
        private static readonly Color BgColor = Color.FromArgb(14, 22, 40);
        private static readonly Color SepColor = Color.FromArgb(22, 38, 68);

        // Referencia al ListView para leer los anchos de columna en tiempo real
        public ListView? LinkedListView { get; set; }

        // Nombres y alineación de las columnas
        private static readonly string[] Names = { "#", "Archivo", "Duración", "Tamaño" };
        private static readonly bool[] Center = { true, false, true, true };

        public PlaylistColumnHeader()
        {
            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw,
                true);

            this.BackColor = BgColor;
            this.Height = 24;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            e.Graphics.Clear(BgColor);
        }

        // ── Dibujar columnas ──────────────────────────────────────────────
        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            var rc = ClientRectangle;

            // Fondo completo (cubre hasta el último píxel)
            g.Clear(BgColor);

            // Línea azul inferior
            using var linePen = new Pen(Theme.AccentDim, 1f);
            g.DrawLine(linePen, 0, rc.Height - 1, rc.Width, rc.Height - 1);

            if (LinkedListView == null || LinkedListView.Columns.Count == 0) return;

            using var sepPen = new Pen(SepColor, 1f);

            int x = 0;
            for (int i = 0; i < LinkedListView.Columns.Count; i++)
            {
                int w = LinkedListView.Columns[i].Width;
                if (w <= 0) continue;

                // Separador vertical (excepto la primera columna)
                if (i > 0)
                    g.DrawLine(sepPen, x, 4, x, rc.Height - 4);

                // Texto de la columna
                var label = i < Names.Length ? Names[i] : "";
                var textRc = new Rectangle(x + 6, 0, w - 8, rc.Height);
                var flags = TextFormatFlags.VerticalCenter |
                             TextFormatFlags.EndEllipsis |
                             TextFormatFlags.NoPadding |
                             (Center[i]
                                 ? TextFormatFlags.HorizontalCenter
                                 : TextFormatFlags.Left);

                TextRenderer.DrawText(g, label, Theme.FontTitle, textRc, Theme.Accent, flags);

                x += w;
            }
        }
    }
}