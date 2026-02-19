using System.Drawing;
using System.Windows.Forms;

namespace VideoPlayer
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        // ── Controles principales ────────────────────────────────────────────
        private TableLayoutPanel mainLayout;
        private SplitContainer   contentSplit;

        // Panel de video
        private Panel     videoPanel;

        // Panel de propiedades
        private Panel         propertiesPanel;
        private Label         lblPropsHeader;
        private TableLayoutPanel propsTable;

        // Value labels (propiedades)
        private Label lblPropFileName;
        private Label lblPropDuration;
        private Label lblPropFileSize;
        private Label lblPropFormat;
        private Label lblPropResolution;
        private Label lblPropFrameRate;
        private Label lblPropVideoCodec;
        private Label lblPropAudioCodec;
        private Label lblPropAudio;
        private Label lblPropPath;

        // Panel de controles
        private Panel     controlsPanel;
        private TableLayoutPanel controlsLayout;
        private SeekBar   seekBar;
        private Panel     buttonsPanel;
        private FlatButton btnPrev;
        private FlatButton btnStop;
        private FlatButton btnPlayPause;
        private FlatButton btnNext;
        private FlatButton btnMute;
        private VolumeBar  volumeBar;
        private Label      lblVolume;
        private Label      lblTime;
        private ComboBox   cmbSpeed;
        private Label      lblSpeedHint;
        private FlatButton btnShuffle;
        private FlatButton btnRepeat;
        private FlatButton btnFullscreen;

        // Panel de playlist
        private Panel         playlistPanel;
        private Panel         playlistHeader;
        private Label         lblPlaylistTitle;
        private FlatButton    btnAddFiles;
        private FlatButton    btnRemove;
        private FlatButton    btnClearPlaylist;
        private ListView      playlistView;
        private ColumnHeader  colNum;
        private ColumnHeader  colName;
        private ColumnHeader  colDuration;
        private ColumnHeader  colSize;
        private ColumnHeader  colPath;

        // Timer de actualización
        private System.Windows.Forms.Timer updateTimer;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            // ── Instanciar controles ─────────────────────────────────────────
            this.mainLayout       = new TableLayoutPanel();
            this.contentSplit     = new SplitContainer();
            this.videoPanel       = new Panel();
            this.propertiesPanel  = new Panel();
            this.lblPropsHeader   = new Label();
            this.propsTable       = new TableLayoutPanel();

            this.lblPropFileName  = new Label();
            this.lblPropDuration  = new Label();
            this.lblPropFileSize  = new Label();
            this.lblPropFormat    = new Label();
            this.lblPropResolution= new Label();
            this.lblPropFrameRate = new Label();
            this.lblPropVideoCodec= new Label();
            this.lblPropAudioCodec= new Label();
            this.lblPropAudio     = new Label();
            this.lblPropPath      = new Label();

            this.controlsPanel    = new Panel();
            this.controlsLayout   = new TableLayoutPanel();
            this.seekBar          = new SeekBar();
            this.buttonsPanel     = new Panel();
            this.btnPrev          = new FlatButton();
            this.btnStop          = new FlatButton();
            this.btnPlayPause     = new FlatButton();
            this.btnNext          = new FlatButton();
            this.btnMute          = new FlatButton();
            this.volumeBar        = new VolumeBar();
            this.lblVolume        = new Label();
            this.lblTime          = new Label();
            this.cmbSpeed         = new ComboBox();
            this.lblSpeedHint     = new Label();
            this.btnShuffle       = new FlatButton();
            this.btnRepeat        = new FlatButton();
            this.btnFullscreen    = new FlatButton();
            this.playlistPanel    = new Panel();
            this.playlistHeader   = new Panel();
            this.lblPlaylistTitle = new Label();
            this.btnAddFiles      = new FlatButton();
            this.btnRemove        = new FlatButton();
            this.btnClearPlaylist = new FlatButton();
            this.playlistView     = new ListView();
            this.colNum      = new ColumnHeader();
            this.colName     = new ColumnHeader();
            this.colDuration = new ColumnHeader();
            this.colSize     = new ColumnHeader();
            this.colPath     = new ColumnHeader();
            this.updateTimer = new System.Windows.Forms.Timer(this.components);

            // SuspendLayout masivo
            this.mainLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.contentSplit).BeginInit();
            this.contentSplit.SuspendLayout();
            this.propertiesPanel.SuspendLayout();
            this.propsTable.SuspendLayout();
            this.controlsPanel.SuspendLayout();
            this.controlsLayout.SuspendLayout();
            this.buttonsPanel.SuspendLayout();
            this.playlistPanel.SuspendLayout();
            this.playlistHeader.SuspendLayout();
            this.SuspendLayout();

            // ════════════════════════════════════════════════════════════════
            //  mainLayout
            // ════════════════════════════════════════════════════════════════
            this.mainLayout.Dock        = DockStyle.Fill;
            this.mainLayout.ColumnCount = 1;
            this.mainLayout.RowCount    = 3;
            this.mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this.mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));   // video+props
            this.mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 92F));   // controles
            this.mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 192F));  // playlist
            this.mainLayout.BackColor   = Theme.Background;
            this.mainLayout.Padding     = new Padding(6);
            this.mainLayout.Controls.Add(this.contentSplit,   0, 0);
            this.mainLayout.Controls.Add(this.controlsPanel,  0, 1);
            this.mainLayout.Controls.Add(this.playlistPanel,  0, 2);

            // ════════════════════════════════════════════════════════════════
            //  contentSplit  (izq = video | der = propiedades)
            // ════════════════════════════════════════════════════════════════
            this.contentSplit.Dock              = DockStyle.Fill;
            this.contentSplit.Orientation       = Orientation.Vertical;
            this.contentSplit.SplitterDistance  = 660;
            this.contentSplit.SplitterWidth     = 5;
            this.contentSplit.BackColor         = Theme.Border;
            this.contentSplit.Panel1.Controls.Add(this.videoPanel);
            this.contentSplit.Panel2.Controls.Add(this.propertiesPanel);

            // ════════════════════════════════════════════════════════════════
            //  videoPanel
            // ════════════════════════════════════════════════════════════════
            this.videoPanel.Dock      = DockStyle.Fill;
            this.videoPanel.BackColor = Theme.VideoBlack;

            // ════════════════════════════════════════════════════════════════
            //  propertiesPanel
            // ════════════════════════════════════════════════════════════════
            this.propertiesPanel.Dock      = DockStyle.Fill;
            this.propertiesPanel.BackColor = Theme.Surface;
            this.propertiesPanel.Padding   = new Padding(10, 8, 8, 8);
            this.propertiesPanel.Controls.Add(this.propsTable);
            this.propertiesPanel.Controls.Add(this.lblPropsHeader);

            // Header propiedades
            this.lblPropsHeader.Text      = "PROPIEDADES";
            this.lblPropsHeader.Font      = Theme.FontTitle;
            this.lblPropsHeader.ForeColor = Theme.Accent;
            this.lblPropsHeader.BackColor = Theme.Surface;
            this.lblPropsHeader.Dock      = DockStyle.Top;
            this.lblPropsHeader.Height    = 26;
            this.lblPropsHeader.TextAlign = ContentAlignment.MiddleLeft;
            this.lblPropsHeader.Padding   = new Padding(0, 2, 0, 2);

            // propsTable
            this.propsTable.Dock        = DockStyle.Fill;
            this.propsTable.ColumnCount = 2;
            this.propsTable.RowCount    = 10;
            this.propsTable.BackColor   = Theme.Surface;
            this.propsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90F));
            this.propsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            for (int i = 0; i < 10; i++)
                this.propsTable.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            this.propsTable.Padding   = new Padding(0, 4, 0, 0);

            // Filas de propiedades: key labels (col 0) + value labels (col 1)
            string[] keys = { "Archivo:", "Duración:", "Tamaño:", "Formato:", "Resolución:", "FPS:", "Video codec:", "Audio codec:", "Audio:", "Ruta:" };
            Label[]  vals = { lblPropFileName, lblPropDuration, lblPropFileSize, lblPropFormat, lblPropResolution, lblPropFrameRate, lblPropVideoCodec, lblPropAudioCodec, lblPropAudio, lblPropPath };

            for (int i = 0; i < keys.Length; i++)
            {
                var keyLbl = new Label
                {
                    Text      = keys[i],
                    Font      = Theme.FontSmall,
                    ForeColor = Theme.TextSecondary,
                    BackColor = Theme.Surface,
                    TextAlign = ContentAlignment.MiddleRight,
                    Dock      = DockStyle.Fill,
                    Padding   = new Padding(0, 0, 6, 0)
                };

                vals[i].Text      = "—";
                vals[i].Font      = Theme.FontSmall;
                vals[i].ForeColor = Theme.TextPrimary;
                vals[i].BackColor = Theme.Surface;
                vals[i].TextAlign = ContentAlignment.MiddleLeft;
                vals[i].Dock      = DockStyle.Fill;
                vals[i].AutoEllipsis = true;

                this.propsTable.Controls.Add(keyLbl, 0, i);
                this.propsTable.Controls.Add(vals[i], 1, i);
            }

            // ════════════════════════════════════════════════════════════════
            //  controlsPanel
            // ════════════════════════════════════════════════════════════════
            this.controlsPanel.Dock      = DockStyle.Fill;
            this.controlsPanel.BackColor = Theme.Surface;
            this.controlsPanel.Padding   = new Padding(4, 5, 4, 5);
            this.controlsPanel.Margin    = new Padding(0, 4, 0, 0);
            this.controlsPanel.Controls.Add(this.controlsLayout);

            // controlsLayout: 2 filas (seek bar + botones)
            this.controlsLayout.Dock        = DockStyle.Fill;
            this.controlsLayout.ColumnCount = 1;
            this.controlsLayout.RowCount    = 2;
            this.controlsLayout.BackColor   = Theme.Surface;
            this.controlsLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));
            this.controlsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            this.controlsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this.controlsLayout.Controls.Add(this.seekBar,      0, 0);
            this.controlsLayout.Controls.Add(this.buttonsPanel, 0, 1);

            // seekBar
            this.seekBar.Dock    = DockStyle.Fill;
            this.seekBar.Maximum = 1000;

            // buttonsPanel  (manual layout de botones)
            this.buttonsPanel.Dock      = DockStyle.Fill;
            this.buttonsPanel.BackColor = Theme.Surface;

            // --- Botones de reproducción (centro izquierdo) ---
            int bW = 38, bH = 34, bY = 5, gap = 4;

            // btnPrev  ⏮
            this.btnPrev.Text    = "⏮";
            this.btnPrev.Size    = new Size(bW, bH);
            this.btnPrev.Location= new Point(6, bY);
            this.btnPrev.Click  += (s, e) => btnPrev_Click(s, e);

            // btnStop  ■
            this.btnStop.Text    = "■";
            this.btnStop.Size    = new Size(bW, bH);
            this.btnStop.Location= new Point(6 + (bW + gap), bY);
            this.btnStop.Click  += (s, e) => btnStop_Click(s, e);

            // btnPlayPause  ▶
            this.btnPlayPause.Text     = "▶";
            this.btnPlayPause.Size     = new Size(bW + 10, bH);
            this.btnPlayPause.Location = new Point(6 + (bW + gap) * 2, bY);
            this.btnPlayPause.Font     = new Font("Segoe UI Symbol", 11f, FontStyle.Bold, GraphicsUnit.Point);
            this.btnPlayPause.Click   += (s, e) => btnPlayPause_Click(s, e);

            // btnNext  ⏭
            this.btnNext.Text     = "⏭";
            this.btnNext.Size     = new Size(bW, bH);
            this.btnNext.Location = new Point(6 + (bW + gap) * 2 + 14 + gap, bY);
            this.btnNext.Click   += (s, e) => btnNext_Click(s, e);

            // Mute
            int muteX = 6 + (bW + gap) * 3 + 14 + gap * 2 + 16;
            this.btnMute.Text     = "🔊";
            this.btnMute.Size     = new Size(bW, bH);
            this.btnMute.Location = new Point(muteX, bY);
            this.btnMute.Click   += (s, e) => btnMute_Click(s, e);

            // volumeBar
            this.volumeBar.Value    = 100;
            this.volumeBar.Width    = 90;
            this.volumeBar.Height   = 22;
            this.volumeBar.Location = new Point(muteX + bW + 4, bY + (bH - 22) / 2);
            this.volumeBar.VolumeChanged += (s, e) => OnVolumeBarChanged();

            // lblVolume
            this.lblVolume.Text      = "100";
            this.lblVolume.Font      = Theme.FontSmall;
            this.lblVolume.ForeColor = Theme.TextSecondary;
            this.lblVolume.Size      = new Size(30, bH);
            this.lblVolume.Location  = new Point(muteX + bW + 98, bY);
            this.lblVolume.TextAlign = ContentAlignment.MiddleLeft;

            // lblTime  (derecha del todo)
            this.lblTime.Text      = "0:00 / 0:00";
            this.lblTime.Font      = Theme.FontMono;
            this.lblTime.ForeColor = Theme.TextSecondary;
            this.lblTime.AutoSize  = true;
            this.lblTime.Location  = new Point(muteX + bW + 135, bY + (bH - 16) / 2);

            // cmbSpeed
            this.cmbSpeed.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbSpeed.BackColor     = Theme.Surface2;
            this.cmbSpeed.ForeColor     = Theme.TextPrimary;
            this.cmbSpeed.FlatStyle     = FlatStyle.Flat;
            this.cmbSpeed.Font          = Theme.FontSmall;
            this.cmbSpeed.Width         = 58;
            this.cmbSpeed.Height        = bH;
            this.cmbSpeed.Items.AddRange(new object[] { "0.25×","0.5×","0.75×","1×","1.25×","1.5×","2×","3×" });
            this.cmbSpeed.SelectedIndex = 3;
            this.cmbSpeed.SelectedIndexChanged += (s, e) => OnSpeedChanged();

            // lblSpeedHint
            this.lblSpeedHint.Text      = "vel:";
            this.lblSpeedHint.Font      = Theme.FontSmall;
            this.lblSpeedHint.ForeColor = Theme.TextDim;
            this.lblSpeedHint.AutoSize  = true;

            // btnShuffle  🔀
            this.btnShuffle.Text    = "🔀";
            this.btnShuffle.Size    = new Size(bW, bH);
            this.btnShuffle.Click  += (s, e) => btnShuffle_Click(s, e);

            // btnRepeat  🔁
            this.btnRepeat.Text    = "🔁";
            this.btnRepeat.Size    = new Size(bW, bH);
            this.btnRepeat.Click  += (s, e) => btnRepeat_Click(s, e);

            // btnFullscreen  ⛶
            this.btnFullscreen.Text   = "⛶";
            this.btnFullscreen.Size   = new Size(bW, bH);
            this.btnFullscreen.Click += (s, e) => btnFullscreen_Click(s, e);

            // Anclar automáticamente tiempo, vel y botones a la derecha
            // Se reposicionan en Resize
            this.buttonsPanel.Controls.AddRange(new Control[]
            {
                btnPrev, btnStop, btnPlayPause, btnNext,
                btnMute, volumeBar, lblVolume, lblTime,
                lblSpeedHint, cmbSpeed,
                btnShuffle, btnRepeat, btnFullscreen
            });
            this.buttonsPanel.Resize += (s, e) => LayoutButtonsPanel();

            // ════════════════════════════════════════════════════════════════
            //  playlistPanel
            // ════════════════════════════════════════════════════════════════
            this.playlistPanel.Dock        = DockStyle.Fill;
            this.playlistPanel.BackColor   = Theme.Surface;
            this.playlistPanel.Padding     = new Padding(0, 4, 0, 0);
            this.playlistPanel.Margin      = new Padding(0, 4, 0, 0);
            this.playlistPanel.Controls.Add(this.playlistView);
            this.playlistPanel.Controls.Add(this.playlistHeader);

            // playlistHeader
            this.playlistHeader.Dock      = DockStyle.Top;
            this.playlistHeader.Height    = 32;
            this.playlistHeader.BackColor = Theme.Surface2;
            this.playlistHeader.Padding   = new Padding(4, 0, 4, 0);
            this.playlistHeader.Controls.AddRange(new Control[]
            {
                lblPlaylistTitle, btnAddFiles, btnRemove, btnClearPlaylist
            });

            this.lblPlaylistTitle.Text      = "  LISTA DE REPRODUCCIÓN";
            this.lblPlaylistTitle.Font      = Theme.FontTitle;
            this.lblPlaylistTitle.ForeColor = Theme.Accent;
            this.lblPlaylistTitle.BackColor = Theme.Surface2;
            this.lblPlaylistTitle.Dock      = DockStyle.Left;
            this.lblPlaylistTitle.Width     = 200;
            this.lblPlaylistTitle.TextAlign = ContentAlignment.MiddleLeft;

            int pbW = 90, pbH = 24, pbY = 4;
            this.btnAddFiles.Text     = "+ Agregar";
            this.btnAddFiles.Font     = Theme.FontSmall;
            this.btnAddFiles.Size     = new Size(pbW, pbH);
            this.btnAddFiles.Dock     = DockStyle.Right;
            this.btnAddFiles.Click   += (s, e) => btnAddFiles_Click(s, e);

            this.btnRemove.Text       = "Quitar";
            this.btnRemove.Font       = Theme.FontSmall;
            this.btnRemove.Size       = new Size(70, pbH);
            this.btnRemove.Dock       = DockStyle.Right;
            this.btnRemove.Click     += (s, e) => btnRemove_Click(s, e);

            this.btnClearPlaylist.Text   = "Limpiar";
            this.btnClearPlaylist.Font   = Theme.FontSmall;
            this.btnClearPlaylist.Size   = new Size(70, pbH);
            this.btnClearPlaylist.Dock   = DockStyle.Right;
            this.btnClearPlaylist.Click += (s, e) => btnClearPlaylist_Click(s, e);

            // playlistView (ListView)
            this.playlistView.Dock         = DockStyle.Fill;
            this.playlistView.View         = View.Details;
            this.playlistView.FullRowSelect = true;
            this.playlistView.MultiSelect  = false;
            this.playlistView.HideSelection= false;
            this.playlistView.GridLines    = false;
            this.playlistView.BackColor    = Color.FromArgb(10, 16, 28);
            this.playlistView.ForeColor    = Theme.TextPrimary;
            this.playlistView.BorderStyle  = BorderStyle.None;
            this.playlistView.Font         = Theme.FontNormal;
            this.playlistView.UseCompatibleStateImageBehavior = false;
            this.playlistView.HeaderStyle  = ColumnHeaderStyle.Nonclickable;

            this.colNum.Text       = "#";
            this.colNum.Width      = 38;
            this.colNum.TextAlign  = HorizontalAlignment.Center;
            this.colName.Text      = "Archivo";
            this.colName.Width     = 280;
            this.colDuration.Text  = "Duración";
            this.colDuration.Width = 80;
            this.colDuration.TextAlign = HorizontalAlignment.Center;
            this.colSize.Text      = "Tamaño";
            this.colSize.Width     = 85;
            this.colSize.TextAlign = HorizontalAlignment.Center;
            this.colPath.Text      = "Ruta";
            this.colPath.Width     = 0;

            this.playlistView.Columns.AddRange(new[] { colNum, colName, colDuration, colSize, colPath });

            // ════════════════════════════════════════════════════════════════
            //  Timer
            // ════════════════════════════════════════════════════════════════
            this.updateTimer.Interval = 500;
            this.updateTimer.Tick    += (s, e) => OnUpdateTick();

            // ════════════════════════════════════════════════════════════════
            //  Form
            // ════════════════════════════════════════════════════════════════
            this.Text            = "VideoPlayer";
            this.Size            = new Size(1000, 700);
            this.MinimumSize     = new Size(700, 520);
            this.StartPosition   = FormStartPosition.CenterScreen;
            this.BackColor       = Theme.Background;
            this.KeyPreview      = true;
            this.AllowDrop       = true;
            this.Icon            = SystemIcons.Application;
            this.Controls.Add(this.mainLayout);

            // ResumeLayout
            this.propsTable.ResumeLayout(false);
            this.propertiesPanel.ResumeLayout(false);
            this.controlsLayout.ResumeLayout(false);
            this.buttonsPanel.ResumeLayout(false);
            this.buttonsPanel.PerformLayout();
            this.controlsPanel.ResumeLayout(false);
            this.playlistHeader.ResumeLayout(false);
            this.playlistPanel.ResumeLayout(false);
            this.contentSplit.Panel1.ResumeLayout(false);
            this.contentSplit.Panel2.ResumeLayout(false);
            this.contentSplit.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.contentSplit).EndInit();
            this.mainLayout.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        // ── Helpers de evento (enlazados desde designer) ────────────────────
        partial void btnPrev_Click(object? s, System.EventArgs e);
        partial void btnStop_Click(object? s, System.EventArgs e);
        partial void btnPlayPause_Click(object? s, System.EventArgs e);
        partial void btnNext_Click(object? s, System.EventArgs e);
        partial void btnMute_Click(object? s, System.EventArgs e);
        partial void btnAddFiles_Click(object? s, System.EventArgs e);
        partial void btnRemove_Click(object? s, System.EventArgs e);
        partial void btnClearPlaylist_Click(object? s, System.EventArgs e);
        partial void btnShuffle_Click(object? s, System.EventArgs e);
        partial void btnRepeat_Click(object? s, System.EventArgs e);
        partial void btnFullscreen_Click(object? s, System.EventArgs e);
        partial void OnVolumeBarChanged();
        partial void OnSpeedChanged();
        partial void OnUpdateTick();
        partial void LayoutButtonsPanel();
    }
}
