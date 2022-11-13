using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Musmetaniac.Web.Client.Extensions
{
    public static class HttpClientExtensions
    {
        public static Task SendRequestAsync<TResult>(this HttpClient self,
            Func<HttpRequestMessage> requestMessageFactory,
            Action<TResult>? successCallback = null,
            Action<string>? failureCallback = null,
            Action? finallyCallback = null,
            CancellationToken? cancellationToken = null)
            where TResult : class
        {
            return HttpRequestUtil.SendAsync(self, requestMessageFactory, successCallback, failureCallback, finallyCallback, cancellationToken);
        }
    }
}
