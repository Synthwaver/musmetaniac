using System.Net.Http;

namespace Musmetaniac.Web.Client.Extensions
{
    public static class HttpClientExtensions
    {
        public static PollingJob<TResult> Poll<TResult>(this HttpClient self, PollingJob<TResult>.Options options) where TResult : class
        {
            return new PollingJob<TResult>(self, options);
        }
    }
}
