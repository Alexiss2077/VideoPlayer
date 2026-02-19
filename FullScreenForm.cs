// ═══════════════════════════════════════════════════════════════════════════
//  FullscreenForm.cs  —  NO tiene archivo .Designer.cs asociado.
//  Si Visual Studio generó un FullscreenForm.Designer.cs automáticamente,
//  ELIMÍNALO del proyecto (clic derecho → Eliminar en el Explorador).
//  Esta clase se configura 100% por código y no necesita diseñador visual.
// ═══════════════════════════════════════════════════════════════════════════

using System;
using System.Drawing;
using System.Windows.Forms;

namespace VideoPlayer
{
    // ──────────────────────────────────────────────────────────────────────
    // NO es "partial" → VS no intentará buscar/crear un .Designer.cs
    // ──────────────────────────────────────────────────────────────────────
    internal sealed class FullscreenForm : Form
    {
        // Evento que notifica a Form1 para restaurar el VideoView
        public event EventHandler? FullscreenExited;

        private readonly System.Windows.Forms.Timer _hideCursorTimer;
        private bool _cursorHidden;

        // ──────────────────────────────────────────────────────────────────
        public FullscreenForm()
        {
            // Toda la configuración se hace aquí, sin InitializeComponent()
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

            _hideCursorTimer = new System.Windows.Forms.Timer();
            _hideCursorTimer.Interval = 2500;
            _hideCursorTimer.Tick += HideCursorTimer_Tick;

            this.KeyDown += FullscreenForm_KeyDown;
            this.MouseMove += FullscreenForm_MouseMove;
            this.DoubleClick += FullscreenForm_DoubleClick;

            this.ResumeLayout(false);
        }

        // ── Ocultar cursor por inactividad ────────────────────────────────
        private void HideCursorTimer_Tick(object? sender, EventArgs e)
        {
            _hideCursorTimer.Stop();
            if (!_cursorHidden)
            {
                Cursor.Hide();
                _cursorHidden = true;
            }
        }

        private void FullscreenForm_MouseMove(object? sender, MouseEventArgs e)
        {
            if (_cursorHidden)
            {
                Cursor.Show();
                _cursorHidden = false;
            }
            _hideCursorTimer.Stop();
            _hideCursorTimer.Start();
        }

        // ── Teclas ────────────────────────────────────────────────────────
        private void FullscreenForm_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape || e.KeyCode == Keys.F11)
            {
                e.Handled = true;
                RequestExit();
            }
        }

        private void FullscreenForm_DoubleClick(object? sender, EventArgs e)
        {
            RequestExit();
        }

        // ── Salida controlada ─────────────────────────────────────────────
        public void RequestExit()
        {
            ShowCursorIfHidden();
            _hideCursorTimer.Stop();
            FullscreenExited?.Invoke(this, EventArgs.Empty);
        }

        private void ShowCursorIfHidden()
        {
            if (_cursorHidden)
            {
                Cursor.Show();
                _cursorHidden = false;
            }
        }

        // Impedir cierre accidental con Alt+F4; redirigir a RequestExit
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
                ShowCursorIfHidden();
                _hideCursorTimer.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}