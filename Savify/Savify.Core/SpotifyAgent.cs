using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Savify.Core
{
    public static class SpotifyAgent
    {
        private static bool authed = false;

        private static SpotifyWebAPI _spotify;

        public static string GetUserId(Uri link)
        {
            string userId = Regex.Match(link.OriginalString, @"(?<=/user/)(\w)+").Value;
            return userId;
        }

        public static string GetPlaylistId(Uri link)
        {
            string playlistId = Regex.Match(link.OriginalString, @"(?<=/playlist/)(\w)+").Value;
            return playlistId;
        }

        public static string GetAlbumId(Uri link)
        {
            string albumId = Regex.Match(link.OriginalString, @"(?<=/album/)(\w)+").Value;
            return albumId;
        }

        public static string GetTrackId(Uri link)
        {
            string trackId = Regex.Match(link.OriginalString, @"(?<=/track/)(\w)+").Value;
            return trackId;
        }


        private static void CheckAuth()
        {
            if (!authed)
                Auth();
        }

        public static async void Auth()
        {
            _spotify = new SpotifyWebAPI()
            {
                UseAuth = false,
            };

            WebAPIFactory webApiFactory = new WebAPIFactory(
               "http://localhost",
               8000,
               Properties.Settings.Default.SpotifyClientID,
               Scope.UserReadPrivate,
               TimeSpan.FromSeconds(20)
            );

            try
            {
                _spotify = await webApiFactory.GetWebApi();
                authed = true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            if (_spotify == null)
                return;
        }

        public static SpotifyLinkType ResolveLink(Uri link)
        {
            if (link.OriginalString.Contains("/album/"))
                return SpotifyLinkType.Album;
            else if (link.OriginalString.Contains("/playlist/"))
                return SpotifyLinkType.Playlist;
            else if (link.OriginalString.Contains("/track/"))
                return SpotifyLinkType.Track;
            else
                throw new Exception("Not a valid spotify link.");
        }

        public static List<Track> GetPlaylistTracks(string userId, string playlistId)
        {
            CheckAuth();
            Paging<PlaylistTrack> playlist = _spotify.GetPlaylistTracks(userId, playlistId);
            List<Track> tracks = new List<Track>();

            foreach (PlaylistTrack track in playlist.Items)
            {
                string artists = "";
                foreach (var artist in track.Track.Artists)
                {
                    artists += (artists.Length > 0 ? ", " : "") + artist.Name;
                }
                track.Track.ExternUrls.TryGetValue("spotify", out string link);

                tracks.Add(new Track()
                {
                    SongTitle = track.Track.Name,
                    Album = track.Track.Album.Name,
                    Artists = artists,
                    TrackNumber = track.Track.TrackNumber,
                    DiscNumber = track.Track.DiscNumber,
                    Year = track.Track.Album.ReleaseDate,
                    CoverLink = new Uri(track.Track.Album.Images[0].Url),
                    Status = -1,
                    Progress = 0,
                    Platform = Agent.Spotify,
                    Link = new Uri(link)
                    
                });          
            }
            return tracks;
        }

        public static Track GetTrack(string trackId)
        {
            FullTrack track = _spotify.GetTrack(trackId);

            string artists = "";
            foreach (var artist in track.Artists)
            {
                artists += (artists.Length > 0 ? ", " : "") + artist.Name;
            }

            return new Track()
            {
                SongTitle = track.Name,
                Album = track.Album.Name,
                Artists = artists,
                TrackNumber = track.TrackNumber,
                DiscNumber = track.DiscNumber,
                Year = track.Album.ReleaseDate,
                CoverLink = new Uri(track.Album.Images[0].Url),
                Status = -1,
                Progress = 0,
                Platform = Agent.Spotify
            };
        }

        public static List<Track> GetAlbumTracks(string albumId)
        {
            Paging<SimpleTrack> album = _spotify.GetAlbumTracks(albumId);
            FullAlbum albumInfo = _spotify.GetAlbum(albumId);
            List<Track> tracks = new List<Track>();

            foreach (SimpleTrack track in album.Items)
            {
                string artists = "";
                track.Artists.ForEach(artist => artists += artist.Name + ",");

                tracks.Add(new Track()
                {
                    SongTitle = track.Name,
                    Album = albumInfo.Name,
                    Artists = artists,
                    TrackNumber = track.TrackNumber,
                    DiscNumber = track.DiscNumber,
                    Year = albumInfo.ReleaseDate,
                    CoverLink = new Uri(albumInfo.Images[0].Url),
                    Status = -1,
                    Progress = 0,
                    Platform = Agent.Spotify
                });
            }
            return tracks;
        }
    }
}
