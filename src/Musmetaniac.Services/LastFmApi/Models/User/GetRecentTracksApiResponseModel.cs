using System.Collections.Generic;
using Newtonsoft.Json;

#nullable disable warnings

namespace Musmetaniac.Services.LastFmApi.Models.User
{
    public class GetRecentTracksApiResponseModel
    {
        public RecentTracksModel RecentTracks { get; set; }

        public class RecentTracksModel
        {
            [JsonProperty("track")]
            public IReadOnlyCollection<Track> Tracks { get; set; }
        }

        public class Track
        {
            public string Name { get; set; }
            public Artist Artist { get; set; }
            public Album Album { get; set; }
            public string Url { get; set; }

            [JsonProperty("@attr")]
            public AttributesModel? Attributes { get; set; }
        }

        public class Artist
        {
            [JsonProperty("#text")]
            public string Text { get; set; }
        }

        public class Album
        {
            [JsonProperty("#text")]
            public string Text { get; set; }
        }

        public class AttributesModel
        {
            public bool NowPlaying { get; set; }
        }
    }
}
