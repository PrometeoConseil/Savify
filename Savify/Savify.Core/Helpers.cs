using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Savify.Core
{
    public static class Helpers
    {
        public static Agent GetAgent(Uri url)
        {
            string host = url.Host;

            if (host.Contains(GetEnumDescription(Agent.Spotify)))
                return Agent.Spotify;
            else if (host.Contains(GetEnumDescription(Agent.YouTube)))
                return Agent.YouTube;
            else if (host.Contains(GetEnumDescription(Agent.SoundCloud)))
                return Agent.SoundCloud;
            else
                throw new Exception("Link not supported.");
        }

        public static string GetEnumDescription(Enum value)
        {
            FieldInfo info = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])info.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }


        public static void AddMp3Tags(Uri filePath, Track track)
        {
            TagLib.Id3v2.Tag.DefaultVersion = 3;
            TagLib.Id3v2.Tag.ForceDefaultVersion = true;
            TagLib.File file = TagLib.File.Create(filePath.LocalPath);
            SetAlbumArt(track.CoverLink, file);
            file.Tag.Title = track.SongTitle;
            file.Tag.Performers = track.Artists.Split(',');
            file.Tag.Album = track.Album;
            file.Tag.Track = (uint)track.TrackNumber;
            file.Tag.Year = (uint)Convert.ToInt32(Regex.Match(track.Year, @"(\d)(\d)(\d)(\d)").Value);
            file.RemoveTags(file.TagTypes & ~file.TagTypesOnDisk);
            file.Save();
        }

        public static void SetAlbumArt(Uri link, TagLib.File file)
        {
            byte[] imageBytes;
            using (WebClient client = new WebClient())
            {
                imageBytes = client.DownloadData(link);
            }

            TagLib.Id3v2.AttachedPictureFrame cover = new TagLib.Id3v2.AttachedPictureFrame
            {
                Type = TagLib.PictureType.FrontCover,
                Description = "Cover",
                MimeType = System.Net.Mime.MediaTypeNames.Image.Jpeg,
                Data = imageBytes,
                TextEncoding = TagLib.StringType.UTF16


            };
            file.Tag.Pictures = new TagLib.IPicture[] { cover };
            file.Save();
        }
        public static string ToFilePathSafeString(this string source, char replaceChar = '_')
        {
            return Path.GetInvalidFileNameChars().Aggregate(source,
                (current, invalidFileNameChar) => current.Replace(invalidFileNameChar, replaceChar)).TrimEnd(' ','.');
        }
    }
    

}
