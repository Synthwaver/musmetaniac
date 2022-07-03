using System.Collections.Generic;
using Newtonsoft.Json;

namespace Musmetaniac.Services.LastFmApi.Models.Track
{
    public class GetTopTagsApiResponseModel
    {
        public TopTagsModel TopTags { get; set; }

        public class TopTagsModel
        {
            [JsonProperty("tag")]
            public IReadOnlyCollection<Tag> Tags { get; set; }
        }

        public class Tag
        {
            public string Name { get; set; }
            public string Url { get; set; }
        }
    }
}
