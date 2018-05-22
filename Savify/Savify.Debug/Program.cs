using Savify.Core;
using System;
using System.Collections.Generic;

namespace Savify.Debug
{
    class Program
    {
        static void Main(string[] args)
        {
            SpotifyAgent.Auth();

            //Test Playlist
            //Uri link = new Uri("https://open.spotify.com/user/lozza11111/playlist/66Q0HHtNRqqd6Aw79jfPpA?si=n7hwbprgTQu29SWru9b8cg");

            Uri link = new Uri(@"https://open.spotify.com/album/3h5a97Q165hNExBkutPTTp?si=DU6EeA6IQiiLQ5mRLSOhMA");

            DownloadList newList =  DownloadManager.NewList("Test", new Uri(@"D:\Users\Laurence\Desktop\Testing"));
            DownloadManager.AddTracksLink(link, newList);

            newList.TrackFormat = Format.mp3;
            newList.TrackQuality = Quality._320kbps;

            DownloadManager.EnqueueList(newList);
            DownloadManager.DequeueList(newList);
            
            Console.ReadLine();
        }
    }
}
