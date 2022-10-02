using System;
using System.Net.Http;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
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
        private readonly IWebAssemblyHostEnvironment _hostEnvironment;
        private readonly AppSettings _appSettings;
        private readonly Uri _baseUrl;

        public MusmetaniacApiRequestMessageProvider(AppSettings appSettings, IWebAssemblyHostEnvironment hostEnvironment)
        {
            _appSettings = appSettings;
            _hostEnvironment = hostEnvironment;
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
            var requestMessage = new HttpRequestMessage { Method = httpMethod };
            var uriBuilder = new UriBuilder(new Uri(_baseUrl, relativeUrl).ToString());

            if (parameters != null)
            {
                if (httpMethod == HttpMethod.Get)
                    uriBuilder.Query = parameters.ToQueryString().ToUriComponent();
                else
                    requestMessage.Content = new StringContent(parameters.ToJson());
            }

            requestMessage.RequestUri = uriBuilder.Uri;

            if (!_hostEnvironment.IsDevelopment())
                requestMessage.Headers.Add("x-functions-key", _appSettings.MusmetaniacApiAzureFunctionsKey);

            return requestMessage;
        }
    }
}
