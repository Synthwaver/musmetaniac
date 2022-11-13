using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Musmetaniac.Services.Exceptions;
using Musmetaniac.Services.LastFmApi.Models.User;
using Newtonsoft.Json.Linq;

namespace Musmetaniac.Services.LastFmApi.Clients
{
    public class UserClient : ClientBase
    {
        public UserClient(HttpClient httpClient, string apiKey) : base(httpClient, apiKey)
        {
        }

        public async Task<GetRecentTracksModel> GetRecentTracksAsync(GetRecentTracksRequestModel model)
        {
            var parameters = new Dictionary<string, string>
            {
                ["user"] = model.Username,
            };

            if (model.Limit.HasValue)
                parameters["limit"] = model.Limit.Value.ToString();

            if (model.From.HasValue)
                parameters["from"] = new DateTimeOffset(model.From.Value.ToUniversalTime()).ToUnixTimeSeconds().ToString();

            var responseJson = await GetJsonAsync("user.getRecentTracks", parameters);

            var tracksJToken = JToken.Parse(responseJson).SelectToken("recenttracks.track")!;

            if (tracksJToken is not JArray)
                tracksJToken = new JArray(tracksJToken);

            var tracks = tracksJToken.ToObject<GetRecentTracksApiResponseModel.Track[]>()!;

            // Last.fm API is very buggy and can sometimes return a large list of random tracks not related to the user
            if (tracks.Length > model.Limit + 1)
                throw new LastFmApiRequestException("Unexpected number of tracks received.");

            return new GetRecentTracksModel
            {
                Tracks = tracks.Select(t => new GetRecentTracksModel.Track
                {
                    Name = t.Name,
                    Artist = new GetRecentTracksModel.Artist
                    {
                        Name = t.Artist.Text,
                    },
                    Album = new GetRecentTracksModel.Album
                    {
                        Name = t.Album.Text,
                    },
                    Url = t.Url,
                    IsPlayingNow = t.Attributes?.NowPlaying ?? false,
                    ScrobbledAt = t.Date == null ? null : DateTimeOffset.FromUnixTimeSeconds(t.Date.UnixTimeSeconds).UtcDateTime,
                }).ToArray(),
            };
        }
    }
}
