using System.Drawing;
using System.Windows.Forms;

namespace VideoPlayer
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        // ── Controles ────────────────────────────────────────────────────────
        private TableLayoutPanel mainLayout;
        private SplitContainer contentSplit;
        private Panel videoPanel;

        // Propiedades
        private Panel propertiesPanel;
        private Label lblPropsHeader;
        private TableLayoutPanel propsTable;

        // Keys (col 0)
        private Label lblKeyFileName;
        private Label lblKeyDuration;
        private Label lblKeyFileSize;
        private Label lblKeyFormat;
        private Label lblKeyResolution;
        private Label lblKeyFrameRate;
        private Label lblKeyVideoCodec;
        private Label lblKeyAudioCodec;
        private Label lblKeyAudio;
        private Label lblKeyPath;

        // Values (col 1)
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

        // Controles
        private Panel controlsPanel;
        private TableLayoutPanel controlsLayout;
        private SeekBar seekBar;
        private Panel buttonsPanel;
        private FlatButton btnPrev;
        private FlatButton btnStop;
        private FlatButton btnPlayPause;
        private FlatButton btnNext;
        private FlatButton btnMute;
        private VolumeBar volumeBar;
        private Label lblVolume;
        private Label lblTime;
        private ComboBox cmbSpeed;
        private Label lblSpeedHint;
        private FlatButton btnShuffle;
        private FlatButton btnRepeat;
        private FlatButton btnFullscreen;

        // Playlist
        private Panel playlistPanel;
        private Panel playlistHeader;
        private Label lblPlaylistTitle;
        private FlatButton btnAddFiles;
        private FlatButton btnRemove;
        private FlatButton btnClearPlaylist;
        private ListView playlistView;
        private ColumnHeader colNum;
        private ColumnHeader colName;
        private ColumnHeader colDuration;
        private ColumnHeader colSize;
        private ColumnHeader colPath;

        private System.Windows.Forms.Timer updateTimer;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            this.mainLayout = new TableLayoutPanel();
            this.contentSplit = new SplitContainer();
            this.videoPanel = new Panel();
            this.propertiesPanel = new Panel();
            this.lblPropsHeader = new Label();
            this.propsTable = new TableLayoutPanel();
            this.lblKeyFileName = new Label();
            this.lblKeyDuration = new Label();
            this.lblKeyFileSize = new Label();
            this.lblKeyFormat = new Label();
            this.lblKeyResolution = new Label();
            this.lblKeyFrameRate = new Label();
            this.lblKeyVideoCodec = new Label();
            this.lblKeyAudioCodec = new Label();
            this.lblKeyAudio = new Label();
            this.lblKeyPath = new Label();
            this.lblPropFileName = new Label();
            this.lblPropDuration = new Label();
            this.lblPropFileSize = new Label();
            this.lblPropFormat = new Label();
            this.lblPropResolution = new Label();
            this.lblPropFrameRate = new Label();
            this.lblPropVideoCodec = new Label();
            this.lblPropAudioCodec = new Label();
            this.lblPropAudio = new Label();
            this.lblPropPath = new Label();
            this.controlsPanel = new Panel();
            this.controlsLayout = new TableLayoutPanel();
            this.seekBar = new SeekBar();
            this.buttonsPanel = new Panel();
            this.btnPrev = new FlatButton();
            this.btnStop = new FlatButton();
            this.btnPlayPause = new FlatButton();
            this.btnNext = new FlatButton();
            this.btnMute = new FlatButton();
            this.volumeBar = new VolumeBar();
            this.lblVolume = new Label();
            this.lblTime = new Label();
            this.cmbSpeed = new ComboBox();
            this.lblSpeedHint = new Label();
            this.btnShuffle = new FlatButton();
            this.btnRepeat = new FlatButton();
            this.btnFullscreen = new FlatButton();
            this.playlistPanel = new Panel();
            this.playlistHeader = new Panel();
            this.lblPlaylistTitle = new Label();
            this.btnAddFiles = new FlatButton();
            this.btnRemove = new FlatButton();
            this.btnClearPlaylist = new FlatButton();
            this.playlistView = new ListView();
            this.colNum = new ColumnHeader();
            this.colName = new ColumnHeader();
            this.colDuration = new ColumnHeader();
            this.colSize = new ColumnHeader();
            this.colPath = new ColumnHeader();
            this.updateTimer = new System.Windows.Forms.Timer(this.components);

            this.mainLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.contentSplit)).BeginInit();
            this.contentSplit.SuspendLayout();
            this.propertiesPanel.SuspendLayout();
            this.propsTable.SuspendLayout();
            this.controlsPanel.SuspendLayout();
            this.controlsLayout.SuspendLayout();
            this.buttonsPanel.SuspendLayout();
            this.playlistPanel.SuspendLayout();
            this.playlistHeader.SuspendLayout();
            this.SuspendLayout();

            // ── mainLayout ───────────────────────────────────────────────────
            this.mainLayout.Dock = DockStyle.Fill;
            this.mainLayout.ColumnCount = 1;
            this.mainLayout.RowCount = 3;
            this.mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this.mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            this.mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 92F));
            this.mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 192F));
            this.mainLayout.BackColor = Theme.Background;
            this.mainLayout.Padding = new Padding(6);
            this.mainLayout.Controls.Add(this.contentSplit, 0, 0);
            this.mainLayout.Controls.Add(this.controlsPanel, 0, 1);
            this.mainLayout.Controls.Add(this.playlistPanel, 0, 2);

            // ── contentSplit ─────────────────────────────────────────────────
            this.contentSplit.Dock = DockStyle.Fill;
            this.contentSplit.Orientation = Orientation.Vertical;
            this.contentSplit.SplitterDistance = 660;
            this.contentSplit.SplitterWidth = 5;
            this.contentSplit.BackColor = Theme.Border;
            this.contentSplit.Panel1.Controls.Add(this.videoPanel);
            this.contentSplit.Panel2.Controls.Add(this.propertiesPanel);

            // ── videoPanel ───────────────────────────────────────────────────
            this.videoPanel.Dock = DockStyle.Fill;
            this.videoPanel.BackColor = Theme.VideoBlack;

            // ── propertiesPanel ──────────────────────────────────────────────
            this.propertiesPanel.Dock = DockStyle.Fill;
            this.propertiesPanel.BackColor = Theme.Surface;
            this.propertiesPanel.Padding = new Padding(10, 8, 8, 8);
            this.propertiesPanel.Controls.Add(this.propsTable);
            this.propertiesPanel.Controls.Add(this.lblPropsHeader);

            this.lblPropsHeader.Text = "PROPIEDADES";
            this.lblPropsHeader.Font = Theme.FontTitle;
            this.lblPropsHeader.ForeColor = Theme.Accent;
            this.lblPropsHeader.BackColor = Theme.Surface;
            this.lblPropsHeader.Dock = DockStyle.Top;
            this.lblPropsHeader.Height = 26;
            this.lblPropsHeader.TextAlign = ContentAlignment.MiddleLeft;

            // ── propsTable ───────────────────────────────────────────────────
            this.propsTable.Dock = DockStyle.Fill;
            this.propsTable.ColumnCount = 2;
            this.propsTable.RowCount = 10;
            this.propsTable.BackColor = Theme.Surface;
            this.propsTable.Padding = new Padding(0, 4, 0, 0);
            this.propsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90F));
            this.propsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this.propsTable.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            this.propsTable.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            this.propsTable.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            this.propsTable.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            this.propsTable.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            this.propsTable.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            this.propsTable.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            this.propsTable.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            this.propsTable.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            this.propsTable.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));

            // Keys
            this.lblKeyFileName.Text = "Archivo:";
            this.lblKeyFileName.Font = Theme.FontSmall;
            this.lblKeyFileName.ForeColor = Theme.TextSecondary;
            this.lblKeyFileName.BackColor = Theme.Surface;
            this.lblKeyFileName.TextAlign = ContentAlignment.MiddleRight;
            this.lblKeyFileName.Dock = DockStyle.Fill;
            this.lblKeyFileName.Padding = new Padding(0, 0, 6, 0);

            this.lblKeyDuration.Text = "Duración:";
            this.lblKeyDuration.Font = Theme.FontSmall;
            this.lblKeyDuration.ForeColor = Theme.TextSecondary;
            this.lblKeyDuration.BackColor = Theme.Surface;
            this.lblKeyDuration.TextAlign = ContentAlignment.MiddleRight;
            this.lblKeyDuration.Dock = DockStyle.Fill;
            this.lblKeyDuration.Padding = new Padding(0, 0, 6, 0);

            this.lblKeyFileSize.Text = "Tamaño:";
            this.lblKeyFileSize.Font = Theme.FontSmall;
            this.lblKeyFileSize.ForeColor = Theme.TextSecondary;
            this.lblKeyFileSize.BackColor = Theme.Surface;
            this.lblKeyFileSize.TextAlign = ContentAlignment.MiddleRight;
            this.lblKeyFileSize.Dock = DockStyle.Fill;
            this.lblKeyFileSize.Padding = new Padding(0, 0, 6, 0);

            this.lblKeyFormat.Text = "Formato:";
            this.lblKeyFormat.Font = Theme.FontSmall;
            this.lblKeyFormat.ForeColor = Theme.TextSecondary;
            this.lblKeyFormat.BackColor = Theme.Surface;
            this.lblKeyFormat.TextAlign = ContentAlignment.MiddleRight;
            this.lblKeyFormat.Dock = DockStyle.Fill;
            this.lblKeyFormat.Padding = new Padding(0, 0, 6, 0);

            this.lblKeyResolution.Text = "Resolución:";
            this.lblKeyResolution.Font = Theme.FontSmall;
            this.lblKeyResolution.ForeColor = Theme.TextSecondary;
            this.lblKeyResolution.BackColor = Theme.Surface;
            this.lblKeyResolution.TextAlign = ContentAlignment.MiddleRight;
            this.lblKeyResolution.Dock = DockStyle.Fill;
            this.lblKeyResolution.Padding = new Padding(0, 0, 6, 0);

            this.lblKeyFrameRate.Text = "FPS:";
            this.lblKeyFrameRate.Font = Theme.FontSmall;
            this.lblKeyFrameRate.ForeColor = Theme.TextSecondary;
            this.lblKeyFrameRate.BackColor = Theme.Surface;
            this.lblKeyFrameRate.TextAlign = ContentAlignment.MiddleRight;
            this.lblKeyFrameRate.Dock = DockStyle.Fill;
            this.lblKeyFrameRate.Padding = new Padding(0, 0, 6, 0);

            this.lblKeyVideoCodec.Text = "Video codec:";
            this.lblKeyVideoCodec.Font = Theme.FontSmall;
            this.lblKeyVideoCodec.ForeColor = Theme.TextSecondary;
            this.lblKeyVideoCodec.BackColor = Theme.Surface;
            this.lblKeyVideoCodec.TextAlign = ContentAlignment.MiddleRight;
            this.lblKeyVideoCodec.Dock = DockStyle.Fill;
            this.lblKeyVideoCodec.Padding = new Padding(0, 0, 6, 0);

            this.lblKeyAudioCodec.Text = "Audio codec:";
            this.lblKeyAudioCodec.Font = Theme.FontSmall;
            this.lblKeyAudioCodec.ForeColor = Theme.TextSecondary;
            this.lblKeyAudioCodec.BackColor = Theme.Surface;
            this.lblKeyAudioCodec.TextAlign = ContentAlignment.MiddleRight;
            this.lblKeyAudioCodec.Dock = DockStyle.Fill;
            this.lblKeyAudioCodec.Padding = new Padding(0, 0, 6, 0);

            this.lblKeyAudio.Text = "Audio:";
            this.lblKeyAudio.Font = Theme.FontSmall;
            this.lblKeyAudio.ForeColor = Theme.TextSecondary;
            this.lblKeyAudio.BackColor = Theme.Surface;
            this.lblKeyAudio.TextAlign = ContentAlignment.MiddleRight;
            this.lblKeyAudio.Dock = DockStyle.Fill;
            this.lblKeyAudio.Padding = new Padding(0, 0, 6, 0);

            this.lblKeyPath.Text = "Ruta:";
            this.lblKeyPath.Font = Theme.FontSmall;
            this.lblKeyPath.ForeColor = Theme.TextSecondary;
            this.lblKeyPath.BackColor = Theme.Surface;
            this.lblKeyPath.TextAlign = ContentAlignment.MiddleRight;
            this.lblKeyPath.Dock = DockStyle.Fill;
            this.lblKeyPath.Padding = new Padding(0, 0, 6, 0);

            // Values
            this.lblPropFileName.Text = "—";
            this.lblPropFileName.Font = Theme.FontSmall;
            this.lblPropFileName.ForeColor = Theme.TextPrimary;
            this.lblPropFileName.BackColor = Theme.Surface;
            this.lblPropFileName.TextAlign = ContentAlignment.MiddleLeft;
            this.lblPropFileName.Dock = DockStyle.Fill;
            this.lblPropFileName.AutoEllipsis = true;

            this.lblPropDuration.Text = "—";
            this.lblPropDuration.Font = Theme.FontSmall;
            this.lblPropDuration.ForeColor = Theme.TextPrimary;
            this.lblPropDuration.BackColor = Theme.Surface;
            this.lblPropDuration.TextAlign = ContentAlignment.MiddleLeft;
            this.lblPropDuration.Dock = DockStyle.Fill;
            this.lblPropDuration.AutoEllipsis = true;

            this.lblPropFileSize.Text = "—";
            this.lblPropFileSize.Font = Theme.FontSmall;
            this.lblPropFileSize.ForeColor = Theme.TextPrimary;
            this.lblPropFileSize.BackColor = Theme.Surface;
            this.lblPropFileSize.TextAlign = ContentAlignment.MiddleLeft;
            this.lblPropFileSize.Dock = DockStyle.Fill;
            this.lblPropFileSize.AutoEllipsis = true;

            this.lblPropFormat.Text = "—";
            this.lblPropFormat.Font = Theme.FontSmall;
            this.lblPropFormat.ForeColor = Theme.TextPrimary;
            this.lblPropFormat.BackColor = Theme.Surface;
            this.lblPropFormat.TextAlign = ContentAlignment.MiddleLeft;
            this.lblPropFormat.Dock = DockStyle.Fill;
            this.lblPropFormat.AutoEllipsis = true;

            this.lblPropResolution.Text = "—";
            this.lblPropResolution.Font = Theme.FontSmall;
            this.lblPropResolution.ForeColor = Theme.TextPrimary;
            this.lblPropResolution.BackColor = Theme.Surface;
            this.lblPropResolution.TextAlign = ContentAlignment.MiddleLeft;
            this.lblPropResolution.Dock = DockStyle.Fill;
            this.lblPropResolution.AutoEllipsis = true;

            this.lblPropFrameRate.Text = "—";
            this.lblPropFrameRate.Font = Theme.FontSmall;
            this.lblPropFrameRate.ForeColor = Theme.TextPrimary;
            this.lblPropFrameRate.BackColor = Theme.Surface;
            this.lblPropFrameRate.TextAlign = ContentAlignment.MiddleLeft;
            this.lblPropFrameRate.Dock = DockStyle.Fill;
            this.lblPropFrameRate.AutoEllipsis = true;

            this.lblPropVideoCodec.Text = "—";
            this.lblPropVideoCodec.Font = Theme.FontSmall;
            this.lblPropVideoCodec.ForeColor = Theme.TextPrimary;
            this.lblPropVideoCodec.BackColor = Theme.Surface;
            this.lblPropVideoCodec.TextAlign = ContentAlignment.MiddleLeft;
            this.lblPropVideoCodec.Dock = DockStyle.Fill;
            this.lblPropVideoCodec.AutoEllipsis = true;

            this.lblPropAudioCodec.Text = "—";
            this.lblPropAudioCodec.Font = Theme.FontSmall;
            this.lblPropAudioCodec.ForeColor = Theme.TextPrimary;
            this.lblPropAudioCodec.BackColor = Theme.Surface;
            this.lblPropAudioCodec.TextAlign = ContentAlignment.MiddleLeft;
            this.lblPropAudioCodec.Dock = DockStyle.Fill;
            this.lblPropAudioCodec.AutoEllipsis = true;

            this.lblPropAudio.Text = "—";
            this.lblPropAudio.Font = Theme.FontSmall;
            this.lblPropAudio.ForeColor = Theme.TextPrimary;
            this.lblPropAudio.BackColor = Theme.Surface;
            this.lblPropAudio.TextAlign = ContentAlignment.MiddleLeft;
            this.lblPropAudio.Dock = DockStyle.Fill;
            this.lblPropAudio.AutoEllipsis = true;

            this.lblPropPath.Text = "—";
            this.lblPropPath.Font = Theme.FontSmall;
            this.lblPropPath.ForeColor = Theme.TextPrimary;
            this.lblPropPath.BackColor = Theme.Surface;
            this.lblPropPath.TextAlign = ContentAlignment.MiddleLeft;
            this.lblPropPath.Dock = DockStyle.Fill;
            this.lblPropPath.AutoEllipsis = true;

            this.propsTable.Controls.Add(this.lblKeyFileName, 0, 0);
            this.propsTable.Controls.Add(this.lblPropFileName, 1, 0);
            this.propsTable.Controls.Add(this.lblKeyDuration, 0, 1);
            this.propsTable.Controls.Add(this.lblPropDuration, 1, 1);
            this.propsTable.Controls.Add(this.lblKeyFileSize, 0, 2);
            this.propsTable.Controls.Add(this.lblPropFileSize, 1, 2);
            this.propsTable.Controls.Add(this.lblKeyFormat, 0, 3);
            this.propsTable.Controls.Add(this.lblPropFormat, 1, 3);
            this.propsTable.Controls.Add(this.lblKeyResolution, 0, 4);
            this.propsTable.Controls.Add(this.lblPropResolution, 1, 4);
            this.propsTable.Controls.Add(this.lblKeyFrameRate, 0, 5);
            this.propsTable.Controls.Add(this.lblPropFrameRate, 1, 5);
            this.propsTable.Controls.Add(this.lblKeyVideoCodec, 0, 6);
            this.propsTable.Controls.Add(this.lblPropVideoCodec, 1, 6);
            this.propsTable.Controls.Add(this.lblKeyAudioCodec, 0, 7);
            this.propsTable.Controls.Add(this.lblPropAudioCodec, 1, 7);
            this.propsTable.Controls.Add(this.lblKeyAudio, 0, 8);
            this.propsTable.Controls.Add(this.lblPropAudio, 1, 8);
            this.propsTable.Controls.Add(this.lblKeyPath, 0, 9);
            this.propsTable.Controls.Add(this.lblPropPath, 1, 9);

            // ── controlsPanel ────────────────────────────────────────────────
            this.controlsPanel.Dock = DockStyle.Fill;
            this.controlsPanel.BackColor = Theme.Surface;
            this.controlsPanel.Padding = new Padding(4, 5, 4, 5);
            this.controlsPanel.Margin = new Padding(0, 4, 0, 0);
            this.controlsPanel.Controls.Add(this.controlsLayout);

            this.controlsLayout.Dock = DockStyle.Fill;
            this.controlsLayout.ColumnCount = 1;
            this.controlsLayout.RowCount = 2;
            this.controlsLayout.BackColor = Theme.Surface;
            this.controlsLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));
            this.controlsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            this.controlsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this.controlsLayout.Controls.Add(this.seekBar, 0, 0);
            this.controlsLayout.Controls.Add(this.buttonsPanel, 0, 1);

            this.seekBar.Dock = DockStyle.Fill;
            this.seekBar.Maximum = 1000;

            // ── buttonsPanel ─────────────────────────────────────────────────
            this.buttonsPanel.Dock = DockStyle.Fill;
            this.buttonsPanel.BackColor = Theme.Surface;

            this.btnPrev.Text = "⏮";
            this.btnPrev.Size = new Size(38, 34);
            this.btnPrev.Location = new Point(6, 5);
            this.btnPrev.Click += new System.EventHandler(this.BtnPrev_Click);

            this.btnStop.Text = "■";
            this.btnStop.Size = new Size(38, 34);
            this.btnStop.Location = new Point(48, 5);
            this.btnStop.Click += new System.EventHandler(this.BtnStop_Click);

            this.btnPlayPause.Text = "▶";
            this.btnPlayPause.Size = new Size(48, 34);
            this.btnPlayPause.Location = new Point(90, 5);
            this.btnPlayPause.Font = new Font("Segoe UI Symbol", 11f, FontStyle.Bold, GraphicsUnit.Point);
            this.btnPlayPause.Click += new System.EventHandler(this.BtnPlayPause_Click);

            this.btnNext.Text = "⏭";
            this.btnNext.Size = new Size(38, 34);
            this.btnNext.Location = new Point(142, 5);
            this.btnNext.Click += new System.EventHandler(this.BtnNext_Click);

            this.btnMute.Text = "🔊";
            this.btnMute.Size = new Size(38, 34);
            this.btnMute.Location = new Point(196, 5);
            this.btnMute.Click += new System.EventHandler(this.BtnMute_Click);

            this.volumeBar.Value = 100;
            this.volumeBar.Width = 90;
            this.volumeBar.Height = 22;
            this.volumeBar.Location = new Point(238, 11);
            this.volumeBar.VolumeChanged += new System.EventHandler(this.VolumeBar_VolumeChanged);

            this.lblVolume.Text = "100";
            this.lblVolume.Font = Theme.FontSmall;
            this.lblVolume.ForeColor = Theme.TextSecondary;
            this.lblVolume.Size = new Size(30, 34);
            this.lblVolume.Location = new Point(332, 5);
            this.lblVolume.TextAlign = ContentAlignment.MiddleLeft;

            this.lblTime.Text = "0:00 / 0:00";
            this.lblTime.Font = Theme.FontMono;
            this.lblTime.ForeColor = Theme.TextSecondary;
            this.lblTime.AutoSize = true;
            this.lblTime.Location = new Point(368, 12);

            this.lblSpeedHint.Text = "vel:";
            this.lblSpeedHint.Font = Theme.FontSmall;
            this.lblSpeedHint.ForeColor = Theme.TextDim;
            this.lblSpeedHint.AutoSize = true;
            this.lblSpeedHint.Location = new Point(470, 12);

            this.cmbSpeed.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbSpeed.BackColor = Theme.Surface2;
            this.cmbSpeed.ForeColor = Theme.TextPrimary;
            this.cmbSpeed.FlatStyle = FlatStyle.Flat;
            this.cmbSpeed.Font = Theme.FontSmall;
            this.cmbSpeed.Width = 58;
            this.cmbSpeed.Location = new Point(494, 7);
            this.cmbSpeed.Items.AddRange(new object[] { "0.25×", "0.5×", "0.75×", "1×", "1.25×", "1.5×", "2×", "3×" });
            this.cmbSpeed.SelectedIndex = 3;
            this.cmbSpeed.SelectedIndexChanged += new System.EventHandler(this.CmbSpeed_SelectedIndexChanged);

            this.btnShuffle.Text = "🔀";
            this.btnShuffle.Size = new Size(38, 34);
            this.btnShuffle.Location = new Point(558, 5);
            this.btnShuffle.Click += new System.EventHandler(this.BtnShuffle_Click);

            this.btnRepeat.Text = "🔁";
            this.btnRepeat.Size = new Size(38, 34);
            this.btnRepeat.Location = new Point(600, 5);
            this.btnRepeat.Click += new System.EventHandler(this.BtnRepeat_Click);

            this.btnFullscreen.Text = "⛶";
            this.btnFullscreen.Size = new Size(38, 34);
            this.btnFullscreen.Location = new Point(642, 5);
            this.btnFullscreen.Click += new System.EventHandler(this.BtnFullscreen_Click);

            this.buttonsPanel.Controls.Add(this.btnPrev);
            this.buttonsPanel.Controls.Add(this.btnStop);
            this.buttonsPanel.Controls.Add(this.btnPlayPause);
            this.buttonsPanel.Controls.Add(this.btnNext);
            this.buttonsPanel.Controls.Add(this.btnMute);
            this.buttonsPanel.Controls.Add(this.volumeBar);
            this.buttonsPanel.Controls.Add(this.lblVolume);
            this.buttonsPanel.Controls.Add(this.lblTime);
            this.buttonsPanel.Controls.Add(this.lblSpeedHint);
            this.buttonsPanel.Controls.Add(this.cmbSpeed);
            this.buttonsPanel.Controls.Add(this.btnShuffle);
            this.buttonsPanel.Controls.Add(this.btnRepeat);
            this.buttonsPanel.Controls.Add(this.btnFullscreen);
            this.buttonsPanel.Resize += new System.EventHandler(this.ButtonsPanel_Resize);

            // ── playlistPanel ────────────────────────────────────────────────
            this.playlistPanel.Dock = DockStyle.Fill;
            this.playlistPanel.BackColor = Theme.Surface;
            this.playlistPanel.Padding = new Padding(0, 4, 0, 0);
            this.playlistPanel.Margin = new Padding(0, 4, 0, 0);
            this.playlistPanel.Controls.Add(this.playlistView);
            this.playlistPanel.Controls.Add(this.playlistHeader);

            this.playlistHeader.Dock = DockStyle.Top;
            this.playlistHeader.Height = 32;
            this.playlistHeader.BackColor = Theme.Surface2;
            this.playlistHeader.Padding = new Padding(4, 0, 4, 0);
            this.playlistHeader.Controls.Add(this.lblPlaylistTitle);
            this.playlistHeader.Controls.Add(this.btnAddFiles);
            this.playlistHeader.Controls.Add(this.btnRemove);
            this.playlistHeader.Controls.Add(this.btnClearPlaylist);

            this.lblPlaylistTitle.Text = "  LISTA DE REPRODUCCIÓN";
            this.lblPlaylistTitle.Font = Theme.FontTitle;
            this.lblPlaylistTitle.ForeColor = Theme.Accent;
            this.lblPlaylistTitle.BackColor = Theme.Surface2;
            this.lblPlaylistTitle.Dock = DockStyle.Left;
            this.lblPlaylistTitle.Width = 210;
            this.lblPlaylistTitle.TextAlign = ContentAlignment.MiddleLeft;

            this.btnAddFiles.Text = "+ Agregar";
            this.btnAddFiles.Font = Theme.FontSmall;
            this.btnAddFiles.Size = new Size(90, 24);
            this.btnAddFiles.Dock = DockStyle.Right;
            this.btnAddFiles.Click += new System.EventHandler(this.BtnAddFiles_Click);

            this.btnRemove.Text = "Quitar";
            this.btnRemove.Font = Theme.FontSmall;
            this.btnRemove.Size = new Size(70, 24);
            this.btnRemove.Dock = DockStyle.Right;
            this.btnRemove.Click += new System.EventHandler(this.BtnRemove_Click);

            this.btnClearPlaylist.Text = "Limpiar";
            this.btnClearPlaylist.Font = Theme.FontSmall;
            this.btnClearPlaylist.Size = new Size(70, 24);
            this.btnClearPlaylist.Dock = DockStyle.Right;
            this.btnClearPlaylist.Click += new System.EventHandler(this.BtnClearPlaylist_Click);

            this.playlistView.Dock = DockStyle.Fill;
            this.playlistView.View = View.Details;
            this.playlistView.FullRowSelect = true;
            this.playlistView.MultiSelect = false;
            this.playlistView.HideSelection = false;
            this.playlistView.GridLines = false;
            this.playlistView.BackColor = Color.FromArgb(10, 16, 28);
            this.playlistView.ForeColor = Theme.TextPrimary;
            this.playlistView.BorderStyle = BorderStyle.None;
            this.playlistView.Font = Theme.FontNormal;
            this.playlistView.UseCompatibleStateImageBehavior = false;
            this.playlistView.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            this.playlistView.DoubleClick += new System.EventHandler(this.PlaylistView_DoubleClick);
            this.playlistView.KeyDown += new KeyEventHandler(this.PlaylistView_KeyDown);
            this.playlistView.Resize += new System.EventHandler(this.PlaylistView_Resize);

            this.colNum.Text = "#";
            this.colNum.Width = 38;
            this.colNum.TextAlign = HorizontalAlignment.Center;
            this.colName.Text = "Archivo";
            this.colName.Width = 280;
            this.colDuration.Text = "Duración";
            this.colDuration.Width = 80;
            this.colDuration.TextAlign = HorizontalAlignment.Center;
            this.colSize.Text = "Tamaño";
            this.colSize.Width = 85;
            this.colSize.TextAlign = HorizontalAlignment.Center;
            this.colPath.Text = "Ruta";
            this.colPath.Width = 0;

            this.playlistView.Columns.AddRange(new ColumnHeader[] { colNum, colName, colDuration, colSize, colPath });

            // ── Timer ────────────────────────────────────────────────────────
            this.updateTimer.Interval = 500;
            this.updateTimer.Tick += new System.EventHandler(this.UpdateTimer_Tick);

            // ── Form ─────────────────────────────────────────────────────────
            this.Text = "VideoPlayer";
            this.Size = new Size(1000, 700);
            this.MinimumSize = new Size(700, 520);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Theme.Background;
            this.KeyPreview = true;
            this.AllowDrop = true;
            this.DragEnter += new DragEventHandler(this.Form1_DragEnter);
            this.DragDrop += new DragEventHandler(this.Form1_DragDrop);
            this.KeyDown += new KeyEventHandler(this.Form1_KeyDown);
            this.Controls.Add(this.mainLayout);

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
            ((System.ComponentModel.ISupportInitialize)(this.contentSplit)).EndInit();
            this.mainLayout.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}