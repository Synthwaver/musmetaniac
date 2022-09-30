using System;
using System.Collections.Generic;

#nullable disable warnings

namespace Musmetaniac.Services
{
    public class Track
    {
        public string Name { get; set; }
        public string ArtistName { get; set; }
        public string AlbumName { get; set; }
        public string Url { get; set; }
        public bool IsPlayingNow { get; set; }
        public DateTime? ScrobbledAt { get; set; }
        public IReadOnlyCollection<Tag> Tags { get; set; }

        public class Tag
        {
            public string Name { get; set; }
            public string Url { get; set; }
        }
    }
}
