using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Musmetaniac.Services;
using Musmetaniac.Web.Common;
using Musmetaniac.Web.Common.Models;
using Musmetaniac.Web.Serverless.Extensions;

namespace Musmetaniac.Web.Serverless
{
    public class RecentTracksFunction
    {
        private readonly IRecentTracksService _recentTracksService;

        public RecentTracksFunction(IRecentTracksService recentTracksService)
        {
            _recentTracksService = recentTracksService;
        }

        [FunctionName("RecentTracks")]
        public async Task<ActionResult<RecentTracksModel>> Run([HttpTrigger(HttpMethodNames.Get, Route = Routes.RecentTracks)] HttpRequest request)
        {
            var model = request.FromQueryString<RecentTracksRequestModel>();
            var recentTracks = await _recentTracksService.GetRecentTracks(model.Username, model.Limit ?? 5, model.From);

            const int maxTagCount = 5;

            return new RecentTracksModel
            {
                Tracks = recentTracks.Select(t => new RecentTracksModel.Track
                {
                    Name = t.Name,
                    ArtistName = t.ArtistName,
                    AlbumName = t.AlbumName,
                    Url = t.Url,
                    IsPlayingNow = t.IsPlayingNow,
                    ScrobbledAt = t.ScrobbledAt,
                    TopTags = t.Tags.Select(tt => new RecentTracksModel.Tag
                    {
                        Name = tt.Name,
                        Url = tt.Url,
                    }).Take(maxTagCount).ToArray(),
                }).ToArray(),
            };
        }
    }
}
