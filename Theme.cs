using System.Drawing;

namespace VideoPlayer
{
    /// <summary>Paleta de colores del tema oscuro azul-negro.</summary>
    internal static class Theme
    {
        public static readonly Color Background    = Color.FromArgb(8,  12, 20);
        public static readonly Color Surface       = Color.FromArgb(14, 20, 35);
        public static readonly Color Surface2      = Color.FromArgb(18, 27, 48);
        public static readonly Color Accent        = Color.FromArgb(56, 189, 248);   // Azul claro
        public static readonly Color AccentDim     = Color.FromArgb(30, 80, 140);
        public static readonly Color AccentHover   = Color.FromArgb(100, 210, 255);
        public static readonly Color TextPrimary   = Color.FromArgb(224, 234, 248);
        public static readonly Color TextSecondary = Color.FromArgb(100, 120, 160);
        public static readonly Color TextDim       = Color.FromArgb(55,  70, 100);
        public static readonly Color Border        = Color.FromArgb(25,  45, 80);
        public static readonly Color HighlightRow  = Color.FromArgb(22,  50, 90);
        public static readonly Color VideoBlack    = Color.Black;

        // Fuente base
        public static Font FontSmall  => new Font("Segoe UI", 8f,  FontStyle.Regular, GraphicsUnit.Point);
        public static Font FontNormal => new Font("Segoe UI", 9f,  FontStyle.Regular, GraphicsUnit.Point);
        public static Font FontBold   => new Font("Segoe UI", 9f,  FontStyle.Bold,    GraphicsUnit.Point);
        public static Font FontMono   => new Font("Consolas",  9f,  FontStyle.Regular, GraphicsUnit.Point);
        public static Font FontLarge  => new Font("Segoe UI", 10f, FontStyle.Bold,    GraphicsUnit.Point);
        public static Font FontTitle  => new Font("Segoe UI", 8f,  FontStyle.Bold,    GraphicsUnit.Point);
    }
}
