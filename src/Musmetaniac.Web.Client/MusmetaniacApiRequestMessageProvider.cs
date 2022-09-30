using System;
using System.Net.Http;
using Musmetaniac.Common.Extensions;
using Musmetaniac.Web.Common;
using Musmetaniac.Web.Common.Extensions;
using Musmetaniac.Web.Common.Models;

namespace Musmetaniac.Web.Client
{
    public interface IMusmetaniacApiRequestMessageProvider
    {
        HttpRequestMessage GetRecentTracksRequestMessage(string username, int limit, DateTime? from = null);
    }

    public class MusmetaniacApiRequestMessageProvider : IMusmetaniacApiRequestMessageProvider
    {
        private readonly AppSettings _appSettings;
        private readonly Uri _baseUrl;

        public MusmetaniacApiRequestMessageProvider(AppSettings appSettings)
        {
            _appSettings = appSettings;
            _baseUrl = new Uri(_appSettings.MusmetaniacApiBaseUrl);
        }

        public HttpRequestMessage GetRecentTracksRequestMessage(string username, int limit, DateTime? from = null)
        {
            return CreateRequestMessage(HttpMethod.Get, Routes.RecentTracks, new RecentTracksRequestModel
            {
                Username = username,
                Limit = limit,
                From = from,
            });
        }

        private HttpRequestMessage CreateRequestMessage<TParams>(HttpMethod httpMethod, string relativeUrl, TParams? parameters = null) where TParams : class
        {
            var uriBuilder = new UriBuilder(new Uri(_baseUrl, relativeUrl).ToString());

            if (httpMethod == HttpMethod.Get && parameters != null)
                uriBuilder.Query = parameters.ToQueryString().ToUriComponent();

            var requestMessage = new HttpRequestMessage
            {
                Method = httpMethod,
                RequestUri = uriBuilder.Uri,
                Headers = { { "x-functions-key", _appSettings.MusmetaniacApiAzureFunctionsKey } },
            };

            if (httpMethod != HttpMethod.Get && parameters != null)
                requestMessage.Content = new StringContent(parameters.ToJson());

            return requestMessage;
        }
    }
}
