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
            CancellationToken? cancellationToken = null,
            Action<TResult>? successCallback = null,
            Action<string>? failureCallback = null,
            Action? executionStartedCallback = null,
            Action? executionFinishedCallback = null)
            where TResult : class
        {
            return HttpRequestUtil.SendAsync(self, requestMessageFactory, cancellationToken,
                successCallback, failureCallback, executionStartedCallback, executionFinishedCallback);
        }
    }
}
