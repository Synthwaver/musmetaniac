using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Musmetaniac.Web.Client.Extensions;
using Musmetaniac.Web.Common.Models;

namespace Musmetaniac.Web.Client
{
    public static class HttpRequestUtil
    {
        public static async Task SendAsync<TResult>(
            HttpClient httpClient,
            Func<HttpRequestMessage> requestMessageFactory,
            CancellationToken? cancellationToken = null,
            Action<TResult>? successCallback = null,
            Action<string>? failureCallback = null,
            Action? executionStartedCallback = null,
            Action? executionFinishedCallback = null)
            where TResult : class
        {
            executionStartedCallback?.Invoke();
            cancellationToken ??= CancellationToken.None;

            HttpResponseMessage? response = null;
            try
            {
                response = await httpClient.SendAsync(requestMessageFactory(), cancellationToken.Value);
                response.EnsureSuccessStatusCode();

                if (successCallback != null)
                {
                    var result = await response.Content.ReadJsonAsync<TResult>(cancellationToken.Value);
                    successCallback.Invoke(result!);
                }
            }
            catch (OperationCanceledException) when (cancellationToken.Value.IsCancellationRequested)
            {
            }
            catch (HttpRequestException)
            {
                if (failureCallback == null)
                    return;

                string? errorMessage;

                if (response == null)
                    errorMessage = "Network error.";
                else
                {
                    var errorResult = await response.Content.ReadJsonAsync<ErrorResult>(cancellationToken.Value);
                    errorMessage = errorResult?.Message ?? "Unexpected error.";
                }

                failureCallback.Invoke(errorMessage);
            }
            finally
            {
                executionFinishedCallback?.Invoke();
            }
        }
    }
}
