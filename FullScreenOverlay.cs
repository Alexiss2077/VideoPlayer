// ════════════════════════════════════════════════════════════════════════════
//  FullscreenOverlay.cs
//
//  LibVLCSharp crea una ventana Win32 nativa (HWND) para el video.
//  Esa ventana nativa siempre aparece ENCIMA de controles WinForms normales.
//  La única solución es un Form independiente con TopMost = true.
//
//  NO ES "partial" → Visual Studio no generará un .Designer.cs para él.
// ════════════════════════════════════════════════════════════════════════════

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using LibVLCSharp.Shared;

namespace VideoPlayer
{
    internal sealed class FullscreenOverlay : Form
    {
        // ── Altura fija de la barra ───────────────────────────────────────
        public const int BAR_HEIGHT = 88;

        // ── Eventos hacia FullscreenForm / Form1 ──────────────────────────
        public event EventHandler? PlayPauseRequested;
        public event EventHandler? StopRequested;
        public event EventHandler? PrevRequested;
        public event EventHandler? NextRequested;
        public event EventHandler? MuteRequested;
        public event EventHandler? ExitRequested;
        public event EventHandler<float>? SeekRequested;
        public event EventHandler<int>? VolumeChangeRequested;

        // ── Controles ─────────────────────────────────────────────────────
        private readonly SeekBar _seek;
        private readonly FlatButton _btnPrev;
        private readonly FlatButton _btnStop;
        private readonly FlatButton _btnPlay;
        private readonly FlatButton _btnNext;
        private readonly FlatButton _btnMute;
        private readonly FlatButton _btnExit;
        private readonly VolumeBar _vol;
        private readonly Label _lblVol;
        private readonly Label _lblTime;

        // ── Timers ────────────────────────────────────────────────────────
        private readonly System.Windows.Forms.Timer _hideTimer;
        private readonly System.Windows.Forms.Timer _updateTimer;

        // ── Estado ────────────────────────────────────────────────────────
        private readonly LibVLCSharp.Shared.MediaPlayer _player;
        private bool _isSeeking;
        private Screen _screen = Screen.PrimaryScreen!;

        // ════════════════════════════════════════════════════════════════
        public FullscreenOverlay(LibVLCSharp.Shared.MediaPlayer player)
        {
            _player = player;

            // ── Configuración del Form ───────────────────────────────────
            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.BackColor = Color.FromArgb(14, 20, 35);
            this.TransparencyKey = Color.Empty;
            this.KeyPreview = true;
            this.DoubleBuffered = true;
            this.StartPosition = FormStartPosition.Manual;
            this.Height = BAR_HEIGHT;
            this.Name = "FullscreenOverlay";

            // ── Controles ────────────────────────────────────────────────
            _seek = new SeekBar { Maximum = 1000 };
            _btnPrev = Btn("⏮");
            _btnStop = Btn("■");
            _btnPlay = Btn("▶"); _btnPlay.Font = new Font("Segoe UI Symbol", 11f, FontStyle.Bold, GraphicsUnit.Point);
            _btnPlay.Size = new Size(46, 32);
            _btnNext = Btn("⏭");
            _btnMute = Btn("🔊");
            _btnExit = Btn("⊠");

            _vol = new VolumeBar { Width = 82, Height = 20, Value = 100 };

            _lblVol = MkLabel("100", Theme.FontSmall, 28, 20, ContentAlignment.MiddleLeft);
            _lblTime = MkLabel("0:00 / 0:00", Theme.FontMono, 140, 18, ContentAlignment.MiddleLeft);

            // ── Eventos de controles ─────────────────────────────────────
            _btnPlay.Click += (_, _) => PlayPauseRequested?.Invoke(this, EventArgs.Empty);
            _btnStop.Click += (_, _) => StopRequested?.Invoke(this, EventArgs.Empty);
            _btnPrev.Click += (_, _) => PrevRequested?.Invoke(this, EventArgs.Empty);
            _btnNext.Click += (_, _) => NextRequested?.Invoke(this, EventArgs.Empty);
            _btnMute.Click += (_, _) => MuteRequested?.Invoke(this, EventArgs.Empty);
            _btnExit.Click += (_, _) => ExitRequested?.Invoke(this, EventArgs.Empty);

            _seek.SeekStarted += (_, _) => _isSeeking = true;
            _seek.SeekEnded += (_, _) => OnSeekEnded();
            _seek.SeekRequested += (_, _) => { };

            _vol.VolumeChanged += (_, _) =>
            {
                _lblVol.Text = _vol.Value.ToString();
                VolumeChangeRequested?.Invoke(this, _vol.Value);
            };

            this.Controls.AddRange(new Control[]
            {
                _seek, _btnPrev, _btnStop, _btnPlay, _btnNext,
                _btnMute, _vol, _lblVol, _lblTime, _btnExit
            });

            // ── Timer de inactividad ─────────────────────────────────────
            _hideTimer = new System.Windows.Forms.Timer { Interval = 3000 };
            _hideTimer.Tick += (_, _) => HideBar();

            // ── Timer de actualización ───────────────────────────────────
            _updateTimer = new System.Windows.Forms.Timer { Interval = 500 };
            _updateTimer.Tick += UpdateTimer_Tick;
            _updateTimer.Start();

            // Repasar layout al cambiar tamaño
            this.Resize += (_, _) => DoLayout();

            // Mantener activo al hacer clic/mover sobre el overlay
            this.MouseMove += (_, _) => KeepAlive();
            this.MouseClick += (_, _) => KeepAlive();
            foreach (Control c in this.Controls)
            {
                c.MouseMove += (_, _) => KeepAlive();
                c.MouseClick += (_, _) => KeepAlive();
            }
        }

        // ════════════════════════════════════════════════════════════════
        //  Layout
        // ════════════════════════════════════════════════════════════════
        private void DoLayout()
        {
            int w = this.ClientSize.Width;
            int pad = 10;

            // Seek bar — fila superior
            _seek.SetBounds(pad, 6, w - pad * 2, 22);

            // Botones — fila inferior
            int bY = 34, bH = 32, x = pad;

            _btnPrev.SetBounds(x, bY, 38, bH); x += 42;
            _btnStop.SetBounds(x, bY, 38, bH); x += 42;
            _btnPlay.SetBounds(x, bY, 46, bH); x += 52;
            _btnNext.SetBounds(x, bY, 38, bH); x += 48;

            x += 8;
            _btnMute.SetBounds(x, bY, 38, bH); x += 42;
            _vol.SetBounds(x, bY + 6, 82, 20); x += 86;
            _lblVol.SetBounds(x, bY + 6, 28, 20); x += 34;

            x += 8;
            _lblTime.SetBounds(x, bY + 7, 140, 18);

            // Salir — pegado a la derecha
            _btnExit.SetBounds(w - pad - 38, bY, 38, bH);
        }

        // ════════════════════════════════════════════════════════════════
        //  Posicionarse en la pantalla correcta
        // ════════════════════════════════════════════════════════════════
        public void AttachToScreen(Screen screen)
        {
            _screen = screen;
            // Empieza completamente fuera de la pantalla (abajo)
            this.SetBounds(
                screen.Bounds.Left,
                screen.Bounds.Bottom,   // oculto: debajo del borde inferior
                screen.Bounds.Width,
                BAR_HEIGHT);
            DoLayout();
        }

        // ════════════════════════════════════════════════════════════════
        //  Mostrar / ocultar  (inmediato, sin animación para máxima fiabilidad)
        // ════════════════════════════════════════════════════════════════
        public void ShowBar()
        {
            KeepAlive();

            // Posicionar en la parte inferior de la pantalla
            this.SetBounds(
                _screen.Bounds.Left,
                _screen.Bounds.Bottom - BAR_HEIGHT,
                _screen.Bounds.Width,
                BAR_HEIGHT);

            if (!this.Visible) this.Show();
            DoLayout();
        }

        public void HideBar()
        {
            _hideTimer.Stop();
            this.Top = _screen.Bounds.Bottom;   // fuera de la pantalla
        }

        public void HideBarImmediately()
        {
            _hideTimer.Stop();
            _updateTimer.Stop();
            this.Top = _screen.Bounds.Bottom;   // fuera de pantalla
        }

        private void KeepAlive()
        {
            _hideTimer.Stop();
            _hideTimer.Start();
        }

        // ════════════════════════════════════════════════════════════════
        //  API pública de sincronización
        // ════════════════════════════════════════════════════════════════
        public void SetPlaying(bool p) => _btnPlay.Text = p ? "⏸" : "▶";
        public void SetMuted(bool m, int vol) { _btnMute.Text = m ? "🔇" : "🔊"; SetVolume(vol); }
        public void SetVolume(int vol) { _vol.Value = vol; _lblVol.Text = vol.ToString(); }

        // ════════════════════════════════════════════════════════════════
        //  Actualización del seek / tiempo
        // ════════════════════════════════════════════════════════════════
        private void UpdateTimer_Tick(object? sender, EventArgs e)
        {
            if (_player is null) return;
            if (!_isSeeking && _player.Length > 0)
                _seek.Value = (int)((double)_player.Time / _player.Length * _seek.Maximum);

            var ts = TimeSpan.FromMilliseconds(Math.Max(0, _player.Time));
            var dur = TimeSpan.FromMilliseconds(Math.Max(0, _player.Length));
            _lblTime.Text = $"{Fmt(ts)} / {Fmt(dur)}";
        }

        private static string Fmt(TimeSpan t) =>
            t.TotalHours >= 1 ? t.ToString(@"h\:mm\:ss") : t.ToString(@"m\:ss");

        private void OnSeekEnded()
        {
            _isSeeking = false;
            if (_player.Length > 0 && _player.IsSeekable)
                SeekRequested?.Invoke(this, (float)_seek.Value / _seek.Maximum);
        }

        // ════════════════════════════════════════════════════════════════
        //  Fondo degradado
        // ════════════════════════════════════════════════════════════════
        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            var rc = ClientRectangle;
            using var brush = new LinearGradientBrush(
                new Point(0, 0), new Point(0, rc.Height),
                Color.FromArgb(180, 8, 12, 20),
                Color.FromArgb(235, 8, 12, 20));
            g.FillRectangle(brush, rc);

            // Línea azul sutil en el borde superior
            using var pen = new Pen(Theme.AccentDim, 1f);
            g.DrawLine(pen, 0, 0, rc.Width, 0);
        }

        // ════════════════════════════════════════════════════════════════
        //  Helpers de construcción
        // ════════════════════════════════════════════════════════════════
        private static FlatButton Btn(string t) =>
            new FlatButton { Text = t, Size = new Size(38, 32) };

        private static Label MkLabel(string text, Font font, int w, int h, ContentAlignment align) =>
            new Label
            {
                Text = text,
                Font = font,
                ForeColor = Theme.TextSecondary,
                BackColor = Color.Transparent,
                Size = new Size(w, h),
                TextAlign = align
            };

        // ════════════════════════════════════════════════════════════════
        //  Form no debe aparecer en el Alt+Tab ni robar el foco
        // ════════════════════════════════════════════════════════════════
        protected override bool ShowWithoutActivation => true;

        protected override CreateParams CreateParams
        {
            get
            {
                const int WS_EX_NOACTIVATE = 0x08000000;
                const int WS_EX_TOOLWINDOW = 0x00000080;
                var cp = base.CreateParams;
                cp.ExStyle |= WS_EX_NOACTIVATE | WS_EX_TOOLWINDOW;
                return cp;
            }
        }

        // ════════════════════════════════════════════════════════════════
        protected override void Dispose(bool disposing)
        {
            if (disposing) { _hideTimer.Dispose(); _updateTimer.Dispose(); }
            base.Dispose(disposing);
        }
    }
}