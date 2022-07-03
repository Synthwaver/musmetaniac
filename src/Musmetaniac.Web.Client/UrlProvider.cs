using System;
using Microsoft.AspNetCore.Http;
using Musmetaniac.Web.Common;
using Musmetaniac.Web.Common.Extensions;
using Musmetaniac.Web.Common.Models;

namespace Musmetaniac.Web.Client
{
    public static class UrlProvider
    {
        private static readonly Uri BaseUrl = new("http://localhost:7071/api/");

        public static string GetRecentTracksUrl(string username, int limit)
        {
            return GetUrl(Routes.RecentTracks, new RecentTracksRequestModel
            {
                Username = username,
                Limit = limit,
            });
        }

        private static string GetUrl<TParams>(string relativeUrl, TParams queryStringParameters = null) where TParams : class
        {
            return GetUrl(relativeUrl, queryStringParameters.ToQueryString());
        }

        private static string GetUrl(string relativeUrl, QueryString? queryString = null)
        {
            return GetUrl(relativeUrl, queryString?.ToUriComponent());
        }

        private static string GetUrl(string relativeUrl, string queryString = null)
        {
            return new UriBuilder(new Uri(BaseUrl, relativeUrl).ToString())
            {
                Query = queryString,
            }.Uri.ToString();
        }
    }
}
