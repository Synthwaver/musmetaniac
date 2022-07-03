using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Musmetaniac.Services.LastFmApi.Models.Track;

namespace Musmetaniac.Services.LastFmApi.Clients
{
    public class TrackClient : ClientBase
    {
        public TrackClient(HttpClient httpClient, string apiKey) : base(httpClient, apiKey)
        {
        }

        public async Task<GetTopTagsModel> GetTopTagsAsync(GetTopTagsRequestModel model)
        {
            var responseModel = await GetAsync<GetTopTagsApiResponseModel>("track.getTopTags", new Dictionary<string, string>
            {
                ["artist"] = model.Artist,
                ["track"] = model.Track,
            });

            return new GetTopTagsModel
            {
                Tags = responseModel.TopTags.Tags.Select(t => new GetTopTagsModel.Tag
                {
                    Name = t.Name,
                    Url = t.Url,
                }).ToArray(),
            };
        }
    }
}
