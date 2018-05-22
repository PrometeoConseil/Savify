using System;

namespace Savify.Core
{
    public class DownloadTask
    {
        public DownloadTask(Track track)
        {
            switch (track.Platform)
            {
                case Agent.Spotify:
                    break;
                case Agent.YouTube:
                    break;
                case Agent.SoundCloud:
                    break;
                default:
                    break;
            }
        }




        private void YouTubeDownload()
        {
            /*
            if (Format == Format.mp3)
            {
                Helpers.AddMp3Tags(new Uri(OutputPath + OutputName + ".mp3"), );
            }
            */
        }
    }
}
