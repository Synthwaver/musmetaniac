using System;
using System.Net.Http;
using Musmetaniac.Services.LastFmApi.Clients;

namespace Musmetaniac.Services.LastFmApi
{
    public class LastFmApiClient : IDisposable
    {
        private readonly HttpClient _httpClient = new() { BaseAddress = new Uri("http://ws.audioscrobbler.com/2.0/") };

        private readonly string _apiKey;

        private UserClient _userClient;
        private TrackClient _trackClient;

        public UserClient User => _userClient ??= new UserClient(_httpClient, _apiKey);
        public TrackClient Track => _trackClient ??= new TrackClient(_httpClient, _apiKey);

        public LastFmApiClient(string apiKey)
        {
            _apiKey = apiKey;
        }

        public void Dispose()
        {
            _httpClient.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
