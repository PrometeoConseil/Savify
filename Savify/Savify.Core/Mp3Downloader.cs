using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Savify.Core
{
    public class Mp3Downloader
    {
        public Process Agent { get; set; }
        public string BinaryPath { get; set; }

        public string SongTitle { get; set; }
        public string Artist { get; set; }
        public string SearchQuery { get; set; }
        public string OutputPath { get; set; }
        public string OutputName { get; set; }
        public string Format { get; set; }

        public bool Started { get; set; }
        public bool Finished { get; set; }
        public decimal Percentage { get; set; }

        /// <summary>
        /// Searches YouTube for a song using the provided title and artist, then downloadeds the video to the specified destination. This file is then converted to an MP3 of the highest available bitrate.
        /// </summary>
        /// <param name="songTitle">The Title of the song.</param>
        /// <param name="artist">The Artist of the song.</param>
        /// <param name="outputFolder">The destination folder for the downloded song.</param>
        /// <param name="format">The format for the audio to be saved into (3gp, aac, flv, m4a, mp3, mp4, ogg, wav, webm).</param>
        public Mp3Downloader (string songTitle, string artist, string outputFolder, string format)
        {
            Started = false;
            Finished = false;
            Percentage = 0;

            SongTitle = songTitle;
            Artist = artist;
            OutputPath = outputFolder;
            Format = format;

            //Set the YouTube search input and the output file name.
            SearchQuery = string.Format("{0} - {1} Audio", SongTitle, Artist);
            OutputName = string.Format("{0} - {1}", SongTitle, Artist);

            //Checks for the required binaries (youtube-dl.exe and ffmpeg.exe).
            BinaryPath = Path.Combine(Directory.GetCurrentDirectory(), Properties.Settings.Default.BinariesFolderName);
            if (!File.Exists(Path.Combine(BinaryPath, Properties.Settings.Default.YouTubeDl + ".exe")))
            {
                throw new Exception(string.Format("{0}.exe could not be found!", Properties.Settings.Default.YouTubeDl));
            }
            if (!File.Exists(Path.Combine(BinaryPath, Properties.Settings.Default.Ffmpeg + ".exe")))
            {
                throw new Exception(string.Format("{0}.exe could not be found!", Properties.Settings.Default.Ffmpeg));
            }

            //Check if the output directory exists.
            if (!Directory.Exists(OutputPath))
            {
                throw new Exception(string.Format("The path: \"{0}\" cannot be found!", OutputPath));
            }

            //Set the download command for youtube-dl.
            string youtubedlArgs = string.Format(@"--extract-audio --format bestaudio --audio-quality {0} --audio-format {1} --prefer-ffmpeg --ffmpeg-location {2} --continue --restrict-filenames ""ytsearch:\{3}"" --output ""{4}.%(ext)s""", "0", Format, BinaryPath, SearchQuery, OutputPath + OutputName);

            //Create the download process and assign its settings.
            Agent = new Process();
            Agent.StartInfo.UseShellExecute = false;
            Agent.StartInfo.FileName = Path.Combine(BinaryPath, Properties.Settings.Default.YouTubeDl + ".exe");
            Agent.StartInfo.Arguments = youtubedlArgs;
        }

        /// <summary>
        /// Runs the download process.
        /// </summary>
        public void Download()
        {
            Agent.Start();
        }
    }
}
