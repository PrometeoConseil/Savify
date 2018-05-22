using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Savify.Core
{
    public struct Track
    {
        public string SongTitle { get; set; }
        public string Artists { get; set; }
        public string Album { get; set; }
        public int TrackNumber { get; set; }
        public int DiscNumber { get; set; }
        public string Year { get; set; }
        public Uri CoverLink { get; set; }
        public Uri Path { get; set; }
        public int Status { get; set; }
        public decimal Progress { get; set; }
        public Agent Platform { get; set; }
        public Uri Link { get; set; }
    }

    public struct DownloadList
    {
        public string Name { get; set; }
        public List<Track> TrackList { get; set; }
        public Uri DestinationPath { get; set; }
        public decimal Progress { get; set; }
        public Format TrackFormat { get; set; }
        public Quality TrackQuality { get; set; }
    }

    public enum Format
    {
        aac,
        flac,
        mp3,
        m4a,
        opus,
        vorbis,
        wav,
        best
    }

    public enum Agent
    {
        [Description("youtube.com")]
        YouTube,
        [Description("soundcloud.com")]
        SoundCloud,
        [Description("spotify.com")]
        Spotify
    }

    public enum Quality
    {
        [Description("32K")]
        _32kbps = 32,
        [Description("96K")]
        _96kbps = 96,
        [Description("128K")]
        _128kbps = 128,
        [Description("192K")]
        _192kbps = 192,
        [Description("256K")]
        _256kbps = 256,
        [Description("320K")]
        _320kbps = 320,
        [Description("9")]
        lowest = 9,
        [Description("0")]
        highest = 0
    }

    public enum SpotifyLinkType
    {
        Track,
        Playlist,
        Album
    }
}
