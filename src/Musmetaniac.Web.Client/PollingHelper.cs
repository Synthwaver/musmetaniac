using System;
using System.Net.Http;
using Microsoft.Extensions.Logging;

namespace Musmetaniac.Web.Client
{
    public interface IPollingHelper
    {
        PollingJob<TResult> CreatePollingJob<TResult>(Func<HttpRequestMessage> requestMessageFactory, PollingJobOptions<TResult> options) where TResult : class;
    }

    public class PollingHelper : IPollingHelper
    {
        private readonly HttpClient _httpClient;
        private readonly ILoggerFactory _loggerFactory;

        public PollingHelper(HttpClient httpClient, ILoggerFactory loggerFactory)
        {
            _httpClient = httpClient;
            _loggerFactory = loggerFactory;
        }

        public PollingJob<TResult> CreatePollingJob<TResult>(Func<HttpRequestMessage> requestMessageFactory, PollingJobOptions<TResult> options) where TResult : class
        {
            return new PollingJob<TResult>(_httpClient, requestMessageFactory, options, _loggerFactory.CreateLogger<PollingJob<TResult>>());
        }
    }
}
