using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Savify.Core
{
    public static class DownloadManager
    {
        public static List<DownloadList> DownloadLists = new List<DownloadList>();
        public static List<DownloadList> DownloadQueue = new List<DownloadList>();

        public static DownloadList NewList(string name, Uri path)
        {
            if (!Directory.Exists(path.LocalPath))
            {
                Directory.CreateDirectory(path.LocalPath);
            }

            DownloadList newList = new DownloadList
            {
                Name = name,
                DestinationPath = path,
                Progress = 0,
                TrackList = new List<Track>(),               
            };

            DownloadLists.Add(newList);
            return newList;
        }

        public static void AddTracksLink(Uri link, DownloadList list)
        {
            if (Helpers.GetAgent(link) == Agent.Spotify)
            {
                if (SpotifyAgent.ResolveLink(link) == SpotifyLinkType.Playlist)
                {
                    foreach (Track track in SpotifyAgent.GetPlaylistTracks(SpotifyAgent.GetUserId(link), SpotifyAgent.GetPlaylistId(link)))
                    {
                        list.TrackList.Add(track);
                    }
                }

                else if (SpotifyAgent.ResolveLink(link) == SpotifyLinkType.Album)
                {
                    foreach (Track track in SpotifyAgent.GetAlbumTracks(SpotifyAgent.GetAlbumId(link)))
                    {
                        list.TrackList.Add(track);
                    }
                }

                else if (SpotifyAgent.ResolveLink(link) == SpotifyLinkType.Track)
                {
                    list.TrackList.Add(SpotifyAgent.GetTrack(SpotifyAgent.GetTrackId(link)));
                }
            }
        }

        public static void EnqueueList(DownloadList list)
        {
            DownloadQueue.Add(list);
        }

        public static void UnqueueList(DownloadList list)
        {
            DownloadQueue.Remove(list);
        }

        public static void DequeueList(DownloadList list)
        {
            DownloadNextList(DownloadQueue.First());
        }

        public static void DownloadNextList(DownloadList list)
        {
            foreach (Track track in list.TrackList)
            {
                Track refTrack = track;
                if (track.Platform == Agent.Spotify)
                {
                    YouTubeAgent.DownloadSearch(ref refTrack, list.DestinationPath, list.TrackFormat, list.TrackQuality);
                    if (list.TrackFormat == Format.mp3)
                        Helpers.AddMp3Tags(refTrack.Path, refTrack);
                }
            }
        }

        /*
        public static DownloadList GetList(string name)
        {
            foreach (DownloadList list in DownloadLists)
            {
                if (list.Name == name)
                {
                    return list;
                }
            }

            return 
        }
        */
    }
}
