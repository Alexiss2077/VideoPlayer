// ════════════════════════════════════════════════════════════════════════════
//  FullscreenForm.cs  —  NO tiene archivo .Designer.cs asociado.
//  Si Visual Studio generó un FullscreenForm.Designer.cs, ELIMÍNALO.
// ════════════════════════════════════════════════════════════════════════════

using System;
using System.Drawing;
using System.Windows.Forms;
using LibVLCSharp.Shared;

namespace VideoPlayer
{
    internal sealed class FullscreenForm : Form
    {
        // ── Eventos hacia Form1 ───────────────────────────────────────────
        public event EventHandler? FullscreenExited;
        public event EventHandler? PlayPauseRequested;
        public event EventHandler? StopRequested;
        public event EventHandler? PrevRequested;
        public event EventHandler? NextRequested;
        public event EventHandler? MuteRequested;
        public event EventHandler<float>? SeekRequested;
        public event EventHandler<int>? VolumeChangeRequested;

        // ── Overlay ───────────────────────────────────────────────────────
        private readonly FullscreenOverlay _overlay;

        // ─────────────────────────────────────────────────────────────────
        //  Timer de polling del cursor  (evita depender de MouseMove que
        //  VLC bloquea con su HWND nativo)
        // ─────────────────────────────────────────────────────────────────
        private readonly System.Windows.Forms.Timer _cursorPollTimer;
        private readonly System.Windows.Forms.Timer _hideCursorTimer;
        private Point _lastCursorPos = Point.Empty;
        private bool _cursorHidden = false;

        // ════════════════════════════════════════════════════════════════
        public FullscreenForm(LibVLCSharp.Shared.MediaPlayer player,
                              bool isPlaying, bool isMuted, int volume)
        {
            this.SuspendLayout();

            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.Black;
            this.TopMost = true;
            this.ShowInTaskbar = false;
            this.KeyPreview = true;
            this.DoubleBuffered = true;
            this.Text = "VideoPlayer — Pantalla Completa";
            this.Name = "FullscreenForm";

            // ── Overlay ─────────────────────────────────────────────────
            _overlay = new FullscreenOverlay(player);
            _overlay.SetPlaying(isPlaying);
            _overlay.SetMuted(isMuted, volume);

            _overlay.PlayPauseRequested += (_, _) => PlayPauseRequested?.Invoke(this, EventArgs.Empty);
            _overlay.StopRequested += (_, _) => StopRequested?.Invoke(this, EventArgs.Empty);
            _overlay.PrevRequested += (_, _) => PrevRequested?.Invoke(this, EventArgs.Empty);
            _overlay.NextRequested += (_, _) => NextRequested?.Invoke(this, EventArgs.Empty);
            _overlay.MuteRequested += (_, _) => MuteRequested?.Invoke(this, EventArgs.Empty);
            _overlay.ExitRequested += (_, _) => RequestExit();
            _overlay.SeekRequested += (_, p) => SeekRequested?.Invoke(this, p);
            _overlay.VolumeChangeRequested += (_, v) => VolumeChangeRequested?.Invoke(this, v);

            // ── Timer de polling del cursor (cada 80 ms) ─────────────────
            //  VLC captura todos los eventos del mouse con su ventana nativa,
            //  por eso usamos Cursor.Position en vez de MouseMove.
            _cursorPollTimer = new System.Windows.Forms.Timer { Interval = 80 };
            _cursorPollTimer.Tick += CursorPollTimer_Tick;

            // ── Timer ocultar cursor (3 s sin movimiento) ─────────────────
            _hideCursorTimer = new System.Windows.Forms.Timer { Interval = 3000 };
            _hideCursorTimer.Tick += HideCursorTimer_Tick;

            this.KeyDown += FullscreenForm_KeyDown;

            this.ResumeLayout(false);
        }

        // ════════════════════════════════════════════════════════════════
        //  OnLoad — arrancar timers y mostrar overlay
        // ════════════════════════════════════════════════════════════════
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var screen = Screen.FromControl(this);
            _overlay.AttachToScreen(screen);
            _overlay.Show(this);

            _lastCursorPos = Cursor.Position;
            _cursorPollTimer.Start();

            // Mostrar la barra al arrancar para que el usuario sepa que existe
            _overlay.ShowBar();
        }

        // ════════════════════════════════════════════════════════════════
        //  Polling del cursor — detecta movimiento aunque VLC capture el mouse
        // ════════════════════════════════════════════════════════════════
        private void CursorPollTimer_Tick(object? sender, EventArgs e)
        {
            var pos = Cursor.Position;

            if (pos != _lastCursorPos)
            {
                _lastCursorPos = pos;

                // Mostrar cursor si estaba oculto
                if (_cursorHidden) { Cursor.Show(); _cursorHidden = false; }

                // Reiniciar timer de ocultar cursor
                _hideCursorTimer.Stop();
                _hideCursorTimer.Start();

                // Mostrar barra de controles
                _overlay.ShowBar();
            }
        }

        private void HideCursorTimer_Tick(object? sender, EventArgs e)
        {
            _hideCursorTimer.Stop();
            if (!_cursorHidden) { Cursor.Hide(); _cursorHidden = true; }
        }

        // ════════════════════════════════════════════════════════════════
        //  API pública
        // ════════════════════════════════════════════════════════════════
        public void NotifyPlaying(bool p) => _overlay.SetPlaying(p);
        public void NotifyMuted(bool m, int v) => _overlay.SetMuted(m, v);
        public void NotifyVolume(int v) => _overlay.SetVolume(v);

        // ════════════════════════════════════════════════════════════════
        //  Teclado
        // ════════════════════════════════════════════════════════════════
        private void FullscreenForm_KeyDown(object? sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                case Keys.F11:
                    e.Handled = true;
                    RequestExit();
                    break;
                case Keys.Space:
                    PlayPauseRequested?.Invoke(this, EventArgs.Empty);
                    e.Handled = true;
                    break;
            }
        }

        // ════════════════════════════════════════════════════════════════
        //  Salida
        // ════════════════════════════════════════════════════════════════
        public void RequestExit()
        {
            if (_cursorHidden) { Cursor.Show(); _cursorHidden = false; }
            _cursorPollTimer.Stop();
            _hideCursorTimer.Stop();
            _overlay.HideBarImmediately();
            FullscreenExited?.Invoke(this, EventArgs.Empty);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                RequestExit();
                return;
            }
            base.OnFormClosing(e);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_cursorHidden) { Cursor.Show(); _cursorHidden = false; }
                _cursorPollTimer.Dispose();
                _hideCursorTimer.Dispose();
                if (!_overlay.IsDisposed) _overlay.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}