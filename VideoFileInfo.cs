using System;
using System.IO;

namespace VideoPlayer
{
    /// <summary>Modelo que representa la información/propiedades de un archivo de video.</summary>
    public class VideoFileInfo
    {
        public string FileName       { get; set; } = string.Empty;
        public string FilePath       { get; set; } = string.Empty;
        public long   FileSizeBytes  { get; set; }
        public TimeSpan Duration     { get; set; }
        public string VideoCodec     { get; set; } = "N/A";
        public string AudioCodec     { get; set; } = "N/A";
        public uint   VideoWidth     { get; set; }
        public uint   VideoHeight    { get; set; }
        public float  FrameRate      { get; set; }
        public uint   AudioSampleRate{ get; set; }
        public uint   AudioChannels  { get; set; }
        public string Format         { get; set; } = string.Empty;

        // ── Propiedades formateadas ──────────────────────────────────────────
        public string FileSizeFormatted
        {
            get
            {
                if (FileSizeBytes < 1024L)            return $"{FileSizeBytes} B";
                if (FileSizeBytes < 1024L * 1024)     return $"{FileSizeBytes / 1024.0:F1} KB";
                if (FileSizeBytes < 1024L * 1024 * 1024) return $"{FileSizeBytes / (1024.0 * 1024):F1} MB";
                return $"{FileSizeBytes / (1024.0 * 1024 * 1024):F2} GB";
            }
        }

        public string DurationFormatted
        {
            get
            {
                if (Duration == TimeSpan.Zero) return "—";
                return Duration.TotalHours >= 1
                    ? Duration.ToString(@"h\:mm\:ss")
                    : Duration.ToString(@"m\:ss");
            }
        }

        public string ResolutionFormatted =>
            VideoWidth > 0 ? $"{VideoWidth} × {VideoHeight}" : "N/A";

        public string FrameRateFormatted =>
            FrameRate > 0 ? $"{FrameRate:F2} fps" : "N/A";

        public string AudioInfoFormatted =>
            AudioSampleRate > 0 ? $"{AudioSampleRate} Hz · {ChannelsFormatted}" : "N/A";

        private string ChannelsFormatted => AudioChannels switch
        {
            1 => "Mono",
            2 => "Stereo",
            6 => "5.1",
            8 => "7.1",
            _ => $"{AudioChannels} ch"
        };
    }
}
