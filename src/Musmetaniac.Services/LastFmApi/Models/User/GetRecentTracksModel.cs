using System.Collections.Generic;

#nullable disable warnings

namespace Musmetaniac.Services.LastFmApi.Models.User
{
    public class GetRecentTracksModel
    {
        public IReadOnlyCollection<Track> Tracks { get; set; }

        public class Track
        {
            public string Name { get; set; }
            public Artist Artist { get; set; }
            public Album Album { get; set; }
            public string Url { get; set; }
            public bool IsPlayingNow { get; set; }
        }

        public class Artist
        {
            public string Name { get; set; }
        }

        public class Album
        {
            public string Name { get; set; }
        }
    }
}
