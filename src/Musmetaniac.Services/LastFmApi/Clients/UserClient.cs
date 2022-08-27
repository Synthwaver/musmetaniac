﻿using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Musmetaniac.Services.LastFmApi.Models.User;

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
                parameters["limit"] = model.Limit.ToString()!;

            var responseModel = await GetAsync<GetRecentTracksApiResponseModel>("user.getRecentTracks", parameters);

            return new GetRecentTracksModel
            {
                Tracks = responseModel.RecentTracks.Tracks.Select(t => new GetRecentTracksModel.Track
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
                }).ToArray(),
            };
        }
    }
}