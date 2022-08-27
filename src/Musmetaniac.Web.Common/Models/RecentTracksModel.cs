using System.Collections.Generic;

#nullable disable warnings

namespace Musmetaniac.Web.Common.Models
{
    public class RecentTracksModel
    {
        public IReadOnlyCollection<Track> Tracks { get; set; }

        public class Track
        {
            public string Name { get; set; }
            public string ArtistName { get; set; }
            public string AlbumName { get; set; }
            public string Url { get; set; }
            public bool IsPlayingNow { get; set; }
            public IReadOnlyCollection<Tag> TopTags { get; set; }
        }

        public class Tag
        {
            public string Name { get; set; }
            public string Url { get; set; }
        }
    }
}
