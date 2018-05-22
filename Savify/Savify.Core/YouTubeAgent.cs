using System;
using System.Diagnostics;
using System.IO;

namespace Savify.Core
{
    public static class YouTubeAgent
    {
        public static Process _youtube;
        public static string youtubedlArgs;

        /// <summary>
        /// Searches YouTube for a song using the provided title and artist, then downloadeds the video to the specified destination. This file is then converted to an MP3 of the highest available bitrate.
        /// </summary>
        /// <param name="songTitle">The Title of the song.</param>
        /// <param name="artist">The Artist of the song.</param>
        /// <param name="outputFolder">The destination folder for the downloded song.</param>
        /// <param name="format">The format for the audio to be saved into (3gp, aac, flv, m4a, mp3, mp4, ogg, wav, webm).</param>
        public static void DownloadSearch(ref Track track, Uri outputFolder, Format format, Quality quality)
        {
            //Set the YouTube search input and the output file name.
            string searchQuery = string.Format("{0} - {1} Audio", track.SongTitle, track.Artists);
            string outputName = string.Format("{0} - {1}", track.SongTitle, track.Artists);
            string fullPath = string.Format(@"{0}\{1}", outputFolder.LocalPath, outputName);

            //Checks for the required binaries (youtube-dl.exe and ffmpeg.exe).
            String binaryPath = Path.Combine(Directory.GetCurrentDirectory(), Properties.Settings.Default.BinariesFolderName);
            if (!File.Exists(Path.Combine(binaryPath, Properties.Settings.Default.YouTubeDl + ".exe")))
            {
                throw new Exception(string.Format("{0}.exe could not be found!", Properties.Settings.Default.YouTubeDl));
            }
            if (!File.Exists(Path.Combine(binaryPath, Properties.Settings.Default.Ffmpeg + ".exe")))
            {
                throw new Exception(string.Format("{0}.exe could not be found!", Properties.Settings.Default.Ffmpeg));
            }

            //Check if the output directory exists.
            if (!Directory.Exists(outputFolder.LocalPath))
            {
                throw new Exception(string.Format("The path: \"{0}\" cannot be found!", outputFolder));
            }

            //Set the download command for youtube-dl.
            youtubedlArgs = string.Format(@"--extract-audio --format bestaudio --audio-quality {0} --audio-format {1} --prefer-ffmpeg --ffmpeg-location {2} --continue --ignore-errors --restrict-filenames --no-overwrites  ""ytsearch:\{3}"" --output ""{4}.%(ext)s""", Helpers.GetEnumDescription(quality), format, binaryPath, searchQuery, fullPath);

            //Create the download process and assign its settings.
            SetupProcess(Path.Combine(binaryPath, Properties.Settings.Default.YouTubeDl + ".exe"));
            _youtube.Start();
            _youtube.WaitForExit();

            track.Path = new Uri(fullPath + ".mp3");
        }


        public static void SetupProcess(string path)
        {
            _youtube = new Process();
            _youtube.StartInfo.UseShellExecute = false;
            _youtube.StartInfo.FileName = path;
            _youtube.StartInfo.Arguments = youtubedlArgs;
        }
    }
}
