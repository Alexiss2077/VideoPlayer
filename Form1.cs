// ═══════════════════════════════════════════════════════════════════════════
//  VideoPlayer — Form1.cs
//  Reproductor de video minimalista: negro + azul claro
//
//  PAQUETES NuGet requeridos (Tools → NuGet Package Manager):
//      LibVLCSharp              (>=3.9)
//      VideoLAN.LibVLC.Windows  (>=3.0)
//      LibVLCSharp.WinForms     (>=3.9)
//
//  IMPORTANTE: En el .csproj el proyecto debe ser x64:
//      <PlatformTarget>x64</PlatformTarget>
// ═══════════════════════════════════════════════════════════════════════════

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using LibVLCSharp.Shared;
using LibVLCSharp.WinForms;

namespace VideoPlayer
{
    public partial class Form1 : Form
    {
        // ── LibVLC ───────────────────────────────────────────────────────────
        private LibVLC                             _libVLC       = null!;
        private LibVLCSharp.Shared.MediaPlayer     _player       = null!;
        private VideoView                          _videoView    = null!;
        private Media?                             _currentMedia;

        // ── Estado ───────────────────────────────────────────────────────────
        private readonly List<VideoFileInfo> _playlist     = new();
        private int  _currentIndex  = -1;
        private bool _isSeeking     = false;
        private bool _isMuted       = false;
        private bool _shuffleOn     = false;
        private bool _repeatOn      = false;
        private bool _isFullscreen  = false;
        private FormWindowState   _prevState;
        private FormBorderStyle   _prevBorderStyle;
        private int  _prevVolume    = 100;
        private readonly Random   _rng = new();
        private bool _isVlcReady;

        private static readonly float[] SpeedValues = { 0.25f, 0.5f, 0.75f, 1f, 1.25f, 1.5f, 2f, 3f };

        // ────────────────────────────────────────────────────────────────────
        public Form1()
        {
            InitializeComponent();

            if (IsInDesigner())
                return;

            SetupUiEvents();

            if (InitVLC())
            {
                _isVlcReady = true;
                SetupPlayerEvents();
                updateTimer.Start();
                return;
            }

            ShowVideoUnavailableMessage();
        }

        // ═══════════════════════════════════════════════════════════════════
        //  Inicialización VLC
        // ═══════════════════════════════════════════════════════════════════
        private bool InitVLC()
        {
            try
            {
                Core.Initialize();          // SIEMPRE antes de new LibVLC()
                _libVLC = new LibVLC();
                _player = new LibVLCSharp.Shared.MediaPlayer(_libVLC)
                {
                    Volume = 100
                };

                _videoView = new VideoView
                {
                    Dock        = DockStyle.Fill,
                    BackColor   = Color.Black,
                    MediaPlayer = _player
                };
                videoPanel.Controls.Add(_videoView);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static bool IsInDesigner()
        {
            return LicenseManager.UsageMode == LicenseUsageMode.Designtime;
        }

        private void ShowVideoUnavailableMessage()
        {
            var errorLabel = new Label
            {
                Dock = DockStyle.Fill,
                ForeColor = Theme.TextSecondary,
                BackColor = Theme.VideoBlack,
                Font = Theme.FontNormal,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "No se pudo iniciar VLC.\nInstala/restaura los paquetes de LibVLC y ejecuta en x64."
            };
            videoPanel.Controls.Clear();
            videoPanel.Controls.Add(errorLabel);
        }

        private void SetupPlayerEvents()
        {
            // Eventos del reproductor (hilo de LibVLC → invocar al hilo UI)
            _player.Playing      += (_, _) => UI(() => OnVlcPlaying());
            _player.Paused       += (_, _) => UI(() => OnVlcPaused());
            _player.Stopped      += (_, _) => UI(() => OnVlcStopped());
            _player.EndReached   += (_, _) => UI(() => OnVlcEndReached());
            _player.EncounteredError += (_, _) => UI(() => OnVlcError());
        }

        private void SetupUiEvents()
        {
            // Seek bar
            seekBar.SeekStarted  += (_, _) => _isSeeking = true;
            seekBar.SeekEnded    += (_, _) => OnSeekBarReleased();
            seekBar.SeekRequested+= (_, _) => { /* solo actualiza visual */ };

            // Playlist doble-clic y teclas
            playlistView.DoubleClick += (_, _) => PlaySelected();
            playlistView.KeyDown     += PlaylistView_KeyDown;

            // Drag & Drop en el formulario
            this.DragEnter += (_, e) =>
                e.Effect = e.Data?.GetDataPresent(DataFormats.FileDrop) == true
                    ? DragDropEffects.Copy : DragDropEffects.None;
            this.DragDrop += OnFormDragDrop;

            // Teclado global
            this.KeyDown += Form1_KeyDown;

            // columna Nombre responsiva
            playlistView.Resize += (_, _) => ResizePlaylistColumns();
        }

        // ── Hilo seguro ──────────────────────────────────────────────────────
        private void UI(Action a)
        {
            if (IsDisposed || !IsHandleCreated) return;
            if (InvokeRequired) BeginInvoke(a);
            else a();
        }

        // ═══════════════════════════════════════════════════════════════════
        //  Eventos VLC
        // ═══════════════════════════════════════════════════════════════════
        private void OnVlcPlaying()
        {
            btnPlayPause.Text = "⏸";
        }

        private void OnVlcPaused()
        {
            btnPlayPause.Text = "▶";
        }

        private void OnVlcStopped()
        {
            btnPlayPause.Text = "▶";
            seekBar.Value     = 0;
            lblTime.Text      = "0:00 / 0:00";
        }

        private void OnVlcEndReached()
        {
            btnPlayPause.Text = "▶";
            // Pequeño retardo para que VLC libere el media antes del siguiente
            Task.Delay(300).ContinueWith(_ => UI(PlayNext));
        }

        private void OnVlcError()
        {
            MessageBox.Show("Error al reproducir el archivo.\nVerifica que el archivo sea válido.",
                "Error de reproducción", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        // ═══════════════════════════════════════════════════════════════════
        //  Timer de actualización  (cada 500 ms)
        // ═══════════════════════════════════════════════════════════════════
        partial void OnUpdateTick()
        {
            if (!_isVlcReady || _player == null) return;

            // Actualizar barra seek
            if (!_isSeeking && _player.Length > 0)
            {
                long time   = _player.Time;
                long length = _player.Length;
                seekBar.Value = (int)((double)time / length * seekBar.Maximum);
            }

            // Actualizar tiempo
            var ts  = TimeSpan.FromMilliseconds(Math.Max(0, _player.Time));
            var dur = TimeSpan.FromMilliseconds(Math.Max(0, _player.Length));
            lblTime.Text = $"{Fmt(ts)} / {Fmt(dur)}";
        }

        private static string Fmt(TimeSpan t) =>
            t.TotalHours >= 1 ? t.ToString(@"h\:mm\:ss") : t.ToString(@"m\:ss");

        // ═══════════════════════════════════════════════════════════════════
        //  Controles de reproducción
        // ═══════════════════════════════════════════════════════════════════
        partial void btnPlayPause_Click(object? s, EventArgs e)
        {
            if (!_isVlcReady) return;
            if (_currentIndex < 0)
            {
                if (_playlist.Count > 0) PlayAtIndex(0);
                return;
            }

            var state = _player.State;
            switch (state)
            {
                case VLCState.Playing:
                    _player.Pause();
                    break;
                case VLCState.Paused:
                    _player.Pause(); // toggle → reanuda
                    break;
                default:
                    PlayAtIndex(_currentIndex);
                    break;
            }
        }

        partial void btnStop_Click(object? s, EventArgs e)
        {
            if (!_isVlcReady) return;
            Task.Run(() => _player.Stop());
        }

        partial void btnPrev_Click(object? s, EventArgs e) => PlayPrevious();
        partial void btnNext_Click(object? s, EventArgs e) => PlayNext();

        partial void btnMute_Click(object? s, EventArgs e)
        {
            if (!_isVlcReady) return;
            _isMuted = !_isMuted;
            if (_isMuted)
            {
                _prevVolume         = _player.Volume;
                _player.Volume      = 0;
                btnMute.Text        = "🔇";
            }
            else
            {
                _player.Volume      = _prevVolume;
                volumeBar.Value     = _prevVolume;
                btnMute.Text        = "🔊";
            }
        }

        partial void btnShuffle_Click(object? s, EventArgs e)
        {
            _shuffleOn         = !_shuffleOn;
            btnShuffle.IsToggled = _shuffleOn;
            btnShuffle.Invalidate();
        }

        partial void btnRepeat_Click(object? s, EventArgs e)
        {
            _repeatOn          = !_repeatOn;
            btnRepeat.IsToggled  = _repeatOn;
            btnRepeat.Invalidate();
        }

        partial void btnFullscreen_Click(object? s, EventArgs e)
        {
            if (_isFullscreen) ExitFullscreen(); else EnterFullscreen();
        }

        // ── Seek ─────────────────────────────────────────────────────────────
        private void OnSeekBarReleased()
        {
            _isSeeking = false;
            if (!_isVlcReady) return;
            if (_player.Length > 0 && _player.IsSeekable)
            {
                float pos = (float)seekBar.Value / seekBar.Maximum;
                _player.Position = pos;
            }
        }

        // ── Volumen ──────────────────────────────────────────────────────────
        partial void OnVolumeBarChanged()
        {
            if (!_isVlcReady) return;
            _player.Volume  = volumeBar.Value;
            lblVolume.Text  = volumeBar.Value.ToString();
            if (volumeBar.Value == 0)   { _isMuted = true;  btnMute.Text = "🔇"; }
            else                        { _isMuted = false; btnMute.Text = "🔊"; }
        }

        // ── Velocidad ────────────────────────────────────────────────────────
        partial void OnSpeedChanged()
        {
            if (!_isVlcReady) return;
            int idx = cmbSpeed.SelectedIndex;
            if (idx >= 0 && idx < SpeedValues.Length)
                _player.SetRate(SpeedValues[idx]);
        }

        // ═══════════════════════════════════════════════════════════════════
        //  Reproducción de lista
        // ═══════════════════════════════════════════════════════════════════
        private void PlaySelected()
        {
            if (!_isVlcReady) return;
            if (playlistView.SelectedItems.Count == 0) return;
            PlayAtIndex(playlistView.SelectedItems[0].Index);
        }

        private void PlayNext()
        {
            if (!_isVlcReady) return;
            if (_playlist.Count == 0) return;

            int next;
            if (_shuffleOn)
            {
                next = _rng.Next(_playlist.Count);
            }
            else if (_repeatOn)
            {
                next = _currentIndex;
            }
            else
            {
                next = _currentIndex + 1;
                if (next >= _playlist.Count) return; // fin de lista
            }

            PlayAtIndex(next);
        }

        private void PlayPrevious()
        {
            if (!_isVlcReady) return;
            if (_playlist.Count == 0) return;
            // Si han pasado más de 3 s, reiniciar en vez de ir al anterior
            if (_player.Time > 3000)
            {
                _player.Position = 0f;
                return;
            }
            int prev = (_currentIndex - 1 + _playlist.Count) % _playlist.Count;
            PlayAtIndex(prev);
        }

        private void PlayAtIndex(int index)
        {
            if (!_isVlcReady) return;
            if (index < 0 || index >= _playlist.Count) return;
            _currentIndex = index;

            // Resaltar fila actual
            foreach (ListViewItem item in playlistView.Items)
                item.BackColor = Color.FromArgb(10, 16, 28);
            playlistView.Items[index].BackColor = Theme.HighlightRow;
            playlistView.EnsureVisible(index);

            var info   = _playlist[index];
            var media  = new Media(_libVLC, info.FilePath, FromType.FromPath);
            _currentMedia?.Dispose();
            _currentMedia = media;
            _player.Play(media);

            // Título de la ventana
            this.Text = $"VideoPlayer — {info.FileName}";

            // Propiedades
            ShowProperties(info);

            // Asegurarse de que la velocidad sea la seleccionada
            int idx = cmbSpeed.SelectedIndex;
            if (idx >= 0 && idx < SpeedValues.Length)
                _player.SetRate(SpeedValues[idx]);
        }

        // ═══════════════════════════════════════════════════════════════════
        //  Gestión de playlist
        // ═══════════════════════════════════════════════════════════════════
        partial void btnAddFiles_Click(object? s, EventArgs e)
        {
            if (!_isVlcReady) return;
            using var dlg = new OpenFileDialog
            {
                Title       = "Agregar archivos de video",
                Filter      = "Video (*.mp4;*.avi;*.mkv;*.mov;*.wmv;*.flv;*.webm;*.m4v)|" +
                              "*.mp4;*.avi;*.mkv;*.mov;*.wmv;*.flv;*.webm;*.m4v|Todos|*.*",
                Multiselect = true
            };
            if (dlg.ShowDialog() == DialogResult.OK)
                _ = AddFilesAsync(dlg.FileNames);
        }

        partial void btnRemove_Click(object? s, EventArgs e) => RemoveSelected();

        partial void btnClearPlaylist_Click(object? s, EventArgs e)
        {
            if (_isVlcReady) Task.Run(() => _player.Stop());
            _playlist.Clear();
            playlistView.Items.Clear();
            _currentIndex = -1;
            ClearProperties();
            this.Text = "VideoPlayer";
        }

        private async Task AddFilesAsync(IEnumerable<string> paths)
        {
            if (!_isVlcReady) return;

            foreach (var path in paths.Where(IsVideoFile))
            {
                var info = await GetVideoInfoAsync(path);
                _playlist.Add(info);

                var item = new ListViewItem((_playlist.Count).ToString());
                item.SubItems.Add(info.FileName);
                item.SubItems.Add(info.DurationFormatted);
                item.SubItems.Add(info.FileSizeFormatted);
                item.SubItems.Add(info.FilePath);
                item.BackColor = Color.FromArgb(10, 16, 28);
                playlistView.Items.Add(item);
            }
        }

        private async Task<VideoFileInfo> GetVideoInfoAsync(string path)
        {
            var fi   = new FileInfo(path);
            var info = new VideoFileInfo
            {
                FileName      = fi.Name,
                FilePath      = path,
                FileSizeBytes = fi.Length,
                Format        = fi.Extension.TrimStart('.').ToUpper()
            };

            try
            {
                using var media = new Media(_libVLC, path, FromType.FromPath);
                await media.ParseAsync(MediaParseOptions.ParseLocal);

                info.Duration = TimeSpan.FromMilliseconds(media.Duration);

                foreach (var track in media.Tracks)
                {
                    switch (track.TrackType)
                    {
                        case TrackType.Video:
                            var vd = track.Data.Video;
                            info.VideoWidth  = vd.Width;
                            info.VideoHeight = vd.Height;
                            if (vd.FrameRateDen > 0)
                                info.FrameRate = (float)vd.FrameRateNum / vd.FrameRateDen;
                            info.VideoCodec  = FourCC(track.Codec);
                            if (string.IsNullOrWhiteSpace(info.VideoCodec) && track.Description != null)
                                info.VideoCodec = track.Description;
                            break;

                        case TrackType.Audio:
                            var ad = track.Data.Audio;
                            info.AudioSampleRate = ad.Rate;
                            info.AudioChannels   = ad.Channels;
                            info.AudioCodec      = FourCC(track.Codec);
                            if (string.IsNullOrWhiteSpace(info.AudioCodec) && track.Description != null)
                                info.AudioCodec = track.Description;
                            break;
                    }
                }

                // Actualizar duración en la lista
                int idx = _playlist.Count; // índice del ítem que se está añadiendo
                // (se llama antes de agregar, así que es el último)
            }
            catch { /* silencioso: se usa la info parcial */ }

            return info;
        }

        private void RemoveSelected()
        {
            if (playlistView.SelectedItems.Count == 0) return;
            int idx = playlistView.SelectedItems[0].Index;

            if (_isVlcReady && idx == _currentIndex) Task.Run(() => _player.Stop());
            if (idx < _currentIndex) _currentIndex--;

            playlistView.Items.RemoveAt(idx);
            _playlist.RemoveAt(idx);

            // Renumerar
            for (int i = 0; i < playlistView.Items.Count; i++)
                playlistView.Items[i].Text = (i + 1).ToString();
        }

        private static string FourCC(uint code)
        {
            if (code == 0) return "N/A";
            var b = new byte[]
            {
                (byte)( code        & 0xFF),
                (byte)((code >>  8) & 0xFF),
                (byte)((code >> 16) & 0xFF),
                (byte)((code >> 24) & 0xFF)
            };
            var s = System.Text.Encoding.ASCII.GetString(b).Trim('\0', ' ');
            return string.IsNullOrEmpty(s) ? "N/A" : s.ToUpper();
        }

        private static bool IsVideoFile(string path)
        {
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return ext is ".mp4" or ".avi" or ".mkv" or ".mov"
                       or ".wmv" or ".flv" or ".webm" or ".m4v" or ".ts" or ".mpg" or ".mpeg";
        }

        // ═══════════════════════════════════════════════════════════════════
        //  Panel de propiedades
        // ═══════════════════════════════════════════════════════════════════
        private void ShowProperties(VideoFileInfo i)
        {
            lblPropFileName  .Text = i.FileName;
            lblPropDuration  .Text = i.DurationFormatted;
            lblPropFileSize  .Text = i.FileSizeFormatted;
            lblPropFormat    .Text = i.Format;
            lblPropResolution.Text = i.ResolutionFormatted;
            lblPropFrameRate .Text = i.FrameRateFormatted;
            lblPropVideoCodec.Text = i.VideoCodec;
            lblPropAudioCodec.Text = i.AudioCodec;
            lblPropAudio     .Text = i.AudioInfoFormatted;
            lblPropPath      .Text = i.FilePath;

            // Resaltar el label del archivo con color acento
            lblPropFileName.ForeColor = Theme.AccentHover;
        }

        private void ClearProperties()
        {
            foreach (Control c in propsTable.Controls)
                if (propsTable.GetColumn(c) == 1 && c is Label lbl)
                {
                    lbl.Text      = "—";
                    lbl.ForeColor = Theme.TextPrimary;
                }
        }

        // ═══════════════════════════════════════════════════════════════════
        //  Pantalla completa
        // ═══════════════════════════════════════════════════════════════════
        private void EnterFullscreen()
        {
            _prevState       = this.WindowState;
            _prevBorderStyle = this.FormBorderStyle;
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState     = FormWindowState.Maximized;
            _isFullscreen        = true;
            btnFullscreen.Text   = "⊠";
        }

        private void ExitFullscreen()
        {
            this.FormBorderStyle = _prevBorderStyle;
            this.WindowState     = _prevState;
            _isFullscreen        = false;
            btnFullscreen.Text   = "⛶";
        }

        // ═══════════════════════════════════════════════════════════════════
        //  Layout dinámico del panel de botones
        // ═══════════════════════════════════════════════════════════════════
        partial void LayoutButtonsPanel()
        {
            int bW = 38, bH = 34, bY = 5, gap = 4;
            int x = 6;

            // Grupo izquierdo: ⏮ ■ ▶/⏸ ⏭
            btnPrev.    Location = new Point(x, bY);                  x += bW + gap;
            btnStop.    Location = new Point(x, bY);                  x += bW + gap;
            btnPlayPause.Location= new Point(x, bY);  x += bW + 10 + gap;
            btnNext.    Location = new Point(x, bY);                  x += bW + 10;

            // Separador visual (espacio)
            x += 8;

            // Mute + volumen
            btnMute.    Location = new Point(x, bY);                  x += bW + 4;
            volumeBar.  Location = new Point(x, bY + (bH - 22) / 2);  x += 90 + 4;
            lblVolume.  Location = new Point(x, bY);                  x += 32;

            x += 12;

            // Tiempo
            lblTime.    Location = new Point(x, bY + (bH - 14) / 2); x += 110;

            x += 8;

            // Velocidad
            lblSpeedHint.Location= new Point(x, bY + (bH - 14) / 2); x += 28;
            cmbSpeed.   Location = new Point(x, bY + (bH - cmbSpeed.Height) / 2); x += cmbSpeed.Width + 8;

            // Derecha: shuffle | repeat | fullscreen
            int rightX  = buttonsPanel.Width - (bW + gap) * 3 - 6;
            if (rightX  < x) rightX = x;
            btnShuffle.   Location = new Point(rightX, bY);                rightX += bW + gap;
            btnRepeat.    Location = new Point(rightX, bY);                rightX += bW + gap;
            btnFullscreen.Location = new Point(rightX, bY);
        }

        // ═══════════════════════════════════════════════════════════════════
        //  Teclado global
        // ═══════════════════════════════════════════════════════════════════
        private void Form1_KeyDown(object? s, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Space:
                    btnPlayPause_Click(null, EventArgs.Empty);
                    e.Handled = true;
                    break;
                case Keys.Left:
                    if (_isVlcReady && _player.IsSeekable) _player.Time = Math.Max(0, _player.Time - 5000);
                    e.Handled = true;
                    break;
                case Keys.Right:
                    if (_isVlcReady && _player.IsSeekable) _player.Time = Math.Min(_player.Length, _player.Time + 5000);
                    e.Handled = true;
                    break;
                case Keys.Up:
                    volumeBar.Value = Math.Min(100, volumeBar.Value + 5);
                    e.Handled = true;
                    break;
                case Keys.Down:
                    volumeBar.Value = Math.Max(0, volumeBar.Value - 5);
                    e.Handled = true;
                    break;
                case Keys.M:
                    btnMute_Click(null, EventArgs.Empty);
                    e.Handled = true;
                    break;
                case Keys.Escape:
                    if (_isFullscreen) ExitFullscreen();
                    e.Handled = true;
                    break;
                case Keys.F11:
                    btnFullscreen_Click(null, EventArgs.Empty);
                    e.Handled = true;
                    break;
                case Keys.Delete when playlistView.Focused:
                    RemoveSelected();
                    e.Handled = true;
                    break;
            }
        }

        // ═══════════════════════════════════════════════════════════════════
        //  Drag & Drop
        // ═══════════════════════════════════════════════════════════════════
        private void OnFormDragDrop(object? s, DragEventArgs e)
        {
            var files = (string[]?)e.Data?.GetData(DataFormats.FileDrop);
            if (files == null) return;
            var videos = files.Where(IsVideoFile).ToArray();
            if (videos.Length > 0)
                _ = AddFilesAsync(videos);
        }

        // ═══════════════════════════════════════════════════════════════════
        //  Teclas en ListView
        // ═══════════════════════════════════════════════════════════════════
        private void PlaylistView_KeyDown(object? s, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)  { PlaySelected(); e.Handled = true; }
            if (e.KeyCode == Keys.Delete) { RemoveSelected(); e.Handled = true; }
        }

        // ═══════════════════════════════════════════════════════════════════
        //  Responsive ListView column
        // ═══════════════════════════════════════════════════════════════════
        private void ResizePlaylistColumns()
        {
            int fixedWidth = colNum.Width + colDuration.Width + colSize.Width + 20;
            int nameWidth  = Math.Max(120, playlistView.Width - fixedWidth);
            colName.Width  = nameWidth;
        }

        // ═══════════════════════════════════════════════════════════════════
        //  Cierre del formulario
        // ═══════════════════════════════════════════════════════════════════
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            updateTimer.Stop();
            try
            {
                if (_isVlcReady)
                {
                    _player.Stop();
                    _player.Dispose();
                    _currentMedia?.Dispose();
                    _libVLC.Dispose();
                }
            }
            catch { /* silencioso al cerrar */ }
            base.OnFormClosing(e);
        }
    }
}
