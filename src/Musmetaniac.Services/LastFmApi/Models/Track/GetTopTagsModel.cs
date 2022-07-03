using System.Collections.Generic;

namespace Musmetaniac.Services.LastFmApi.Models.Track
{
    public class GetTopTagsModel
    {
        public IReadOnlyCollection<Tag> Tags { get; set; }

        public class Tag
        {
            public string Name { get; set; }
            public string Url { get; set; }
        }
    }
}
