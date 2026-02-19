// ═══════════════════════════════════════════════════════════════════════════
//  VideoPlayer — Form1.cs
//  Motor: LibVLCSharp  (sin WMPLib / AxWMPLib)
//
//  NuGet requeridos:
//      LibVLCSharp              >= 3.9
//      VideoLAN.LibVLC.Windows  >= 3.0
//      LibVLCSharp.WinForms     >= 3.9
//
//  El proyecto debe compilarse como x64 (ya configurado en el .csproj)
// ═══════════════════════════════════════════════════════════════════════════

using System;
using System.Collections.Generic;
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
        private LibVLC _libVLC = null!;
        private LibVLCSharp.Shared.MediaPlayer _player = null!;
        private VideoView _videoView = null!;
        private Media? _currentMedia;

        // ── Estado ───────────────────────────────────────────────────────────
        private readonly List<VideoFileInfo> _playlist = new();
        private int _currentIndex = -1;
        private bool _isSeeking = false;
        private bool _isMuted = false;
        private bool _shuffleOn = false;
        private bool _repeatOn = false;
        private bool _isFullscreen = false;
        private FullscreenForm? _fullscreenForm;
        private int _prevVolume = 100;
        private readonly Random _rng = new();

        private static readonly float[] SpeedValues = { 0.25f, 0.5f, 0.75f, 1f, 1.25f, 1.5f, 2f, 3f };

        // ════════════════════════════════════════════════════════════════════
        public Form1()
        {
            Core.Initialize();          // SIEMPRE antes de new LibVLC()
            InitializeComponent();
            InitVLC();
            WireSeekBarEvents();
            updateTimer.Start();
        }

        // ════════════════════════════════════════════════════════════════════
        //  Inicialización VLC
        // ════════════════════════════════════════════════════════════════════
        private void InitVLC()
        {
            _libVLC = new LibVLC();
            _player = new LibVLCSharp.Shared.MediaPlayer(_libVLC) { Volume = 100 };

            _videoView = new VideoView
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Black,
                MediaPlayer = _player
            };
            videoPanel.Controls.Add(_videoView);

            _player.Playing += (_, _) => UI(OnVlcPlaying);
            _player.Paused += (_, _) => UI(OnVlcPaused);
            _player.Stopped += (_, _) => UI(OnVlcStopped);
            _player.EndReached += (_, _) => Task.Delay(300).ContinueWith(_ => UI(PlayNext));
            _player.EncounteredError += (_, _) => UI(OnVlcError);
        }

        private void WireSeekBarEvents()
        {
            seekBar.SeekStarted += (_, _) => _isSeeking = true;
            seekBar.SeekEnded += (_, _) => OnSeekBarReleased();
            seekBar.SeekRequested += (_, _) => { };
        }

        private void UI(Action a)
        {
            if (IsDisposed || !IsHandleCreated) return;
            if (InvokeRequired) BeginInvoke(a);
            else a();
        }

        // ════════════════════════════════════════════════════════════════════
        //  Eventos VLC
        // ════════════════════════════════════════════════════════════════════
        private void OnVlcPlaying() => btnPlayPause.Text = "⏸";
        private void OnVlcPaused() => btnPlayPause.Text = "▶";

        private void OnVlcStopped()
        {
            btnPlayPause.Text = "▶";
            seekBar.Value = 0;
            lblTime.Text = "0:00 / 0:00";
        }

        private void OnVlcError() =>
            MessageBox.Show("Error al reproducir el archivo.\nVerifica que sea un formato válido.",
                "Error de reproducción", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        // ════════════════════════════════════════════════════════════════════
        //  Timer — UpdateTimer_Tick  (firma requerida por el Designer)
        // ════════════════════════════════════════════════════════════════════
        private void UpdateTimer_Tick(object? sender, EventArgs e)
        {
            if (_player is null) return;

            if (!_isSeeking && _player.Length > 0)
                seekBar.Value = (int)((double)_player.Time / _player.Length * seekBar.Maximum);

            var ts = TimeSpan.FromMilliseconds(Math.Max(0, _player.Time));
            var dur = TimeSpan.FromMilliseconds(Math.Max(0, _player.Length));
            lblTime.Text = $"{Fmt(ts)} / {Fmt(dur)}";
        }

        private static string Fmt(TimeSpan t) =>
            t.TotalHours >= 1 ? t.ToString(@"h\:mm\:ss") : t.ToString(@"m\:ss");

        // ════════════════════════════════════════════════════════════════════
        //  Botones — handlers nombrados (requeridos por el Designer)
        // ════════════════════════════════════════════════════════════════════
        private void BtnPlayPause_Click(object? sender, EventArgs e)
        {
            if (_currentIndex < 0)
            {
                if (_playlist.Count > 0) PlayAtIndex(0);
                return;
            }
            var state = _player.State;
            if (state == VLCState.Playing) _player.Pause();
            else if (state == VLCState.Paused) _player.Pause();
            else PlayAtIndex(_currentIndex);
        }

        private void BtnStop_Click(object? sender, EventArgs e) =>
            Task.Run(() => _player.Stop());

        private void BtnPrev_Click(object? sender, EventArgs e) => PlayPrevious();

        private void BtnNext_Click(object? sender, EventArgs e) => PlayNext();

        private void BtnMute_Click(object? sender, EventArgs e)
        {
            _isMuted = !_isMuted;
            if (_isMuted)
            {
                _prevVolume = _player.Volume;
                _player.Volume = 0;
                btnMute.Text = "🔇";
            }
            else
            {
                _player.Volume = _prevVolume;
                volumeBar.Value = _prevVolume;
                btnMute.Text = "🔊";
            }
        }

        private void BtnShuffle_Click(object? sender, EventArgs e)
        {
            _shuffleOn = !_shuffleOn;
            btnShuffle.IsToggled = _shuffleOn;
            btnShuffle.Invalidate();
        }

        private void BtnRepeat_Click(object? sender, EventArgs e)
        {
            _repeatOn = !_repeatOn;
            btnRepeat.IsToggled = _repeatOn;
            btnRepeat.Invalidate();
        }

        private void BtnFullscreen_Click(object? sender, EventArgs e)
        {
            if (_isFullscreen) ExitFullscreen(); else EnterFullscreen();
        }

        private void BtnAddFiles_Click(object? sender, EventArgs e)
        {
            using var dlg = new OpenFileDialog
            {
                Title = "Agregar archivos de video",
                Filter = "Video (*.mp4;*.avi;*.mkv;*.mov;*.wmv;*.flv;*.webm;*.m4v)|" +
                              "*.mp4;*.avi;*.mkv;*.mov;*.wmv;*.flv;*.webm;*.m4v|Todos|*.*",
                Multiselect = true
            };
            if (dlg.ShowDialog() == DialogResult.OK)
                _ = AddFilesAsync(dlg.FileNames);
        }

        private void BtnRemove_Click(object? sender, EventArgs e) => RemoveSelected();

        private void BtnClearPlaylist_Click(object? sender, EventArgs e)
        {
            Task.Run(() => _player.Stop());
            _playlist.Clear();
            playlistView.Items.Clear();
            _currentIndex = -1;
            ClearProperties();
            this.Text = "VideoPlayer";
        }

        // ════════════════════════════════════════════════════════════════════
        //  Volumen / velocidad
        // ════════════════════════════════════════════════════════════════════
        private void VolumeBar_VolumeChanged(object? sender, EventArgs e)
        {
            _player.Volume = volumeBar.Value;
            lblVolume.Text = volumeBar.Value.ToString();
            _isMuted = volumeBar.Value == 0;
            btnMute.Text = _isMuted ? "🔇" : "🔊";
        }

        private void CmbSpeed_SelectedIndexChanged(object? sender, EventArgs e)
        {
            int idx = cmbSpeed.SelectedIndex;
            if (idx >= 0 && idx < SpeedValues.Length)
                _player.SetRate(SpeedValues[idx]);
        }

        // ════════════════════════════════════════════════════════════════════
        //  Seek
        // ════════════════════════════════════════════════════════════════════
        private void OnSeekBarReleased()
        {
            _isSeeking = false;
            if (_player.Length > 0 && _player.IsSeekable)
                _player.Position = (float)seekBar.Value / seekBar.Maximum;
        }

        // ════════════════════════════════════════════════════════════════════
        //  Navegación de lista
        // ════════════════════════════════════════════════════════════════════
        private void PlayNext()
        {
            if (_playlist.Count == 0) return;
            int next;
            if (_shuffleOn) next = _rng.Next(_playlist.Count);
            else if (_repeatOn) next = _currentIndex;
            else
            {
                next = _currentIndex + 1;
                if (next >= _playlist.Count) return;
            }
            PlayAtIndex(next);
        }

        private void PlayPrevious()
        {
            if (_playlist.Count == 0) return;
            if (_player.Time > 3000) { _player.Position = 0f; return; }
            PlayAtIndex((_currentIndex - 1 + _playlist.Count) % _playlist.Count);
        }

        private void PlayAtIndex(int index)
        {
            if (index < 0 || index >= _playlist.Count) return;
            _currentIndex = index;

            foreach (ListViewItem item in playlistView.Items)
                item.BackColor = Color.FromArgb(10, 16, 28);
            playlistView.Items[index].BackColor = Theme.HighlightRow;
            playlistView.EnsureVisible(index);

            var info = _playlist[index];
            var media = new Media(_libVLC, info.FilePath, FromType.FromPath);
            _currentMedia?.Dispose();
            _currentMedia = media;
            _player.Play(media);

            this.Text = $"VideoPlayer — {info.FileName}";
            ShowProperties(info);

            int idx = cmbSpeed.SelectedIndex;
            if (idx >= 0 && idx < SpeedValues.Length)
                _player.SetRate(SpeedValues[idx]);
        }

        // ════════════════════════════════════════════════════════════════════
        //  Gestión de playlist
        // ════════════════════════════════════════════════════════════════════
        private async Task AddFilesAsync(IEnumerable<string> paths)
        {
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

                if (playlistView.InvokeRequired)
                    playlistView.Invoke(() => playlistView.Items.Add(item));
                else
                    playlistView.Items.Add(item);
            }
        }

        private async Task<VideoFileInfo> GetVideoInfoAsync(string path)
        {
            var fi = new FileInfo(path);
            var info = new VideoFileInfo
            {
                FileName = fi.Name,
                FilePath = path,
                FileSizeBytes = fi.Length,
                Format = fi.Extension.TrimStart('.').ToUpper()
            };

            await Task.Run(() =>
            {
                try
                {
                    using var media = new Media(_libVLC, path, FromType.FromPath);
                    media.Parse(MediaParseOptions.ParseLocal);

                    info.Duration = TimeSpan.FromMilliseconds(media.Duration);

                    foreach (var track in media.Tracks)
                    {
                        switch (track.TrackType)
                        {
                            case TrackType.Video:
                                info.VideoWidth = track.Data.Video.Width;
                                info.VideoHeight = track.Data.Video.Height;
                                if (track.Data.Video.FrameRateDen > 0)
                                    info.FrameRate = (float)track.Data.Video.FrameRateNum
                                                           / track.Data.Video.FrameRateDen;
                                info.VideoCodec = FourCC(track.Codec);
                                if (info.VideoCodec == "N/A" && track.Description != null)
                                    info.VideoCodec = track.Description;
                                break;

                            case TrackType.Audio:
                                info.AudioSampleRate = track.Data.Audio.Rate;
                                info.AudioChannels = track.Data.Audio.Channels;
                                info.AudioCodec = FourCC(track.Codec);
                                if (info.AudioCodec == "N/A" && track.Description != null)
                                    info.AudioCodec = track.Description;
                                break;
                        }
                    }
                }
                catch { /* info parcial en caso de error */ }
            });

            return info;
        }

        private void RemoveSelected()
        {
            if (playlistView.SelectedItems.Count == 0) return;
            int idx = playlistView.SelectedItems[0].Index;

            if (idx == _currentIndex) Task.Run(() => _player.Stop());
            if (idx < _currentIndex) _currentIndex--;

            playlistView.Items.RemoveAt(idx);
            _playlist.RemoveAt(idx);

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
            return string.IsNullOrWhiteSpace(s) ? "N/A" : s.ToUpper();
        }

        private static bool IsVideoFile(string path)
        {
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return ext is ".mp4" or ".avi" or ".mkv" or ".mov"
                       or ".wmv" or ".flv" or ".webm" or ".m4v"
                       or ".ts" or ".mpg" or ".mpeg";
        }

        // ════════════════════════════════════════════════════════════════════
        //  Propiedades
        // ════════════════════════════════════════════════════════════════════
        private void ShowProperties(VideoFileInfo i)
        {
            lblPropFileName.Text = i.FileName;
            lblPropFileName.ForeColor = Theme.AccentHover;
            lblPropDuration.Text = i.DurationFormatted;
            lblPropFileSize.Text = i.FileSizeFormatted;
            lblPropFormat.Text = i.Format;
            lblPropResolution.Text = i.ResolutionFormatted;
            lblPropFrameRate.Text = i.FrameRateFormatted;
            lblPropVideoCodec.Text = i.VideoCodec;
            lblPropAudioCodec.Text = i.AudioCodec;
            lblPropAudio.Text = i.AudioInfoFormatted;
            lblPropPath.Text = i.FilePath;
        }

        private void ClearProperties()
        {
            foreach (Control c in propsTable.Controls)
                if (propsTable.GetColumn(c) == 1 && c is Label lbl)
                {
                    lbl.Text = "—";
                    lbl.ForeColor = Theme.TextPrimary;
                }
        }

        // ════════════════════════════════════════════════════════════════════
        //  Pantalla completa — solo el VIDEO (el Form no se toca)
        // ════════════════════════════════════════════════════════════════════
        private void EnterFullscreen()
        {
            if (_isFullscreen) return;
            _isFullscreen = true;
            btnFullscreen.Text = "⊠";

            videoPanel.Controls.Remove(_videoView);

            _fullscreenForm = new FullscreenForm();
            _fullscreenForm.FullscreenExited += FullscreenForm_FullscreenExited;
            _fullscreenForm.KeyDown += Form1_KeyDown;

            _videoView.Dock = DockStyle.Fill;
            _fullscreenForm.Controls.Add(_videoView);
            _fullscreenForm.Show(this);
        }

        private void FullscreenForm_FullscreenExited(object? sender, EventArgs e)
        {
            ExitFullscreen();
        }

        private void ExitFullscreen()
        {
            if (!_isFullscreen) return;
            _isFullscreen = false;
            btnFullscreen.Text = "⛶";

            if (_fullscreenForm == null) return;

            _fullscreenForm.FullscreenExited -= FullscreenForm_FullscreenExited;
            _fullscreenForm.KeyDown -= Form1_KeyDown;

            _fullscreenForm.Controls.Remove(_videoView);
            _videoView.Dock = DockStyle.Fill;
            videoPanel.Controls.Add(_videoView);

            var fs = _fullscreenForm;
            _fullscreenForm = null;
            try { fs.Close(); fs.Dispose(); } catch { }

            this.Activate();
        }


        // ════════════════════════════════════════════════════════════════════
        //  Layout responsivo de botones — ButtonsPanel_Resize
        // ════════════════════════════════════════════════════════════════════
        private void ButtonsPanel_Resize(object? sender, EventArgs e)
        {
            const int bW = 38, bH = 34, bY = 5, gap = 4;
            int x = 6;

            btnPrev.Location = new Point(x, bY); x += bW + gap;
            btnStop.Location = new Point(x, bY); x += bW + gap;
            btnPlayPause.Location = new Point(x, bY); x += 48 + gap;
            btnNext.Location = new Point(x, bY); x += bW + 10;
            x += 8;

            btnMute.Location = new Point(x, bY); x += bW + 4;
            volumeBar.Location = new Point(x, bY + (bH - 22) / 2); x += 90 + 4;
            lblVolume.Location = new Point(x, bY); x += 34;
            x += 12;

            lblTime.Location = new Point(x, bY + (bH - 14) / 2); x += 115;
            x += 8;

            lblSpeedHint.Location = new Point(x, bY + (bH - 14) / 2); x += 28;
            cmbSpeed.Location = new Point(x, bY + (bH - cmbSpeed.Height) / 2); x += cmbSpeed.Width + 8;

            int rightX = buttonsPanel.Width - (bW + gap) * 3 - 6;
            if (rightX < x) rightX = x;
            btnShuffle.Location = new Point(rightX, bY); rightX += bW + gap;
            btnRepeat.Location = new Point(rightX, bY); rightX += bW + gap;
            btnFullscreen.Location = new Point(rightX, bY);
        }

        // ════════════════════════════════════════════════════════════════════
        //  Teclado — Form1_KeyDown
        // ════════════════════════════════════════════════════════════════════
        private void Form1_KeyDown(object? sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Space:
                    BtnPlayPause_Click(null, EventArgs.Empty);
                    e.Handled = true; break;
                case Keys.Left:
                    if (_player.IsSeekable) _player.Time = Math.Max(0, _player.Time - 5000);
                    e.Handled = true; break;
                case Keys.Right:
                    if (_player.IsSeekable) _player.Time = Math.Min(_player.Length, _player.Time + 5000);
                    e.Handled = true; break;
                case Keys.Up:
                    volumeBar.Value = Math.Min(100, volumeBar.Value + 5);
                    e.Handled = true; break;
                case Keys.Down:
                    volumeBar.Value = Math.Max(0, volumeBar.Value - 5);
                    e.Handled = true; break;
                case Keys.M:
                    BtnMute_Click(null, EventArgs.Empty);
                    e.Handled = true; break;
                case Keys.F11:
                    BtnFullscreen_Click(null, EventArgs.Empty);
                    e.Handled = true; break;
                case Keys.Escape:
                    if (_isFullscreen) ExitFullscreen();
                    e.Handled = true; break;
            }
        }

        // ════════════════════════════════════════════════════════════════════
        //  ListView handlers
        // ════════════════════════════════════════════════════════════════════
        private void PlaylistView_DoubleClick(object? sender, EventArgs e)
        {
            if (playlistView.SelectedItems.Count == 0) return;
            PlayAtIndex(playlistView.SelectedItems[0].Index);
        }

        private void PlaylistView_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) { PlaylistView_DoubleClick(null, EventArgs.Empty); e.Handled = true; }
            if (e.KeyCode == Keys.Delete) { RemoveSelected(); e.Handled = true; }
        }

        private void PlaylistView_Resize(object? sender, EventArgs e)
        {
            int fixedW = colNum.Width + colDuration.Width + colSize.Width + 20;
            colName.Width = Math.Max(120, playlistView.Width - fixedW);
        }

        // ════════════════════════════════════════════════════════════════════
        //  Drag & Drop — Form1_DragEnter / Form1_DragDrop
        // ════════════════════════════════════════════════════════════════════
        private void Form1_DragEnter(object? sender, DragEventArgs e)
        {
            e.Effect = e.Data?.GetDataPresent(DataFormats.FileDrop) == true
                ? DragDropEffects.Copy : DragDropEffects.None;
        }

        private void Form1_DragDrop(object? sender, DragEventArgs e)
        {
            var files = (string[]?)e.Data?.GetData(DataFormats.FileDrop);
            if (files is null) return;
            var videos = files.Where(IsVideoFile).ToArray();
            if (videos.Length > 0) _ = AddFilesAsync(videos);
        }

        // ════════════════════════════════════════════════════════════════════
        //  Cierre
        // ════════════════════════════════════════════════════════════════════
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            updateTimer.Stop();

            // Cerrar ventana fullscreen si está abierta antes de liberar VLC
            if (_isFullscreen && _fullscreenForm != null)
            {
                _isFullscreen = false;
                var fs = _fullscreenForm;
                _fullscreenForm = null;
                try
                {
                    // Recuperar el VideoView para que VLC pueda liberarlo correctamente
                    fs.Controls.Remove(_videoView);
                    videoPanel.Controls.Add(_videoView);
                    fs.Close();
                    fs.Dispose();
                }
                catch { /* silencioso */ }
            }

            try
            {
                _player.Stop();
                _player.Dispose();
                _currentMedia?.Dispose();
                _libVLC.Dispose();
            }
            catch { /* silencioso */ }
            base.OnFormClosing(e);
        }
    }
}