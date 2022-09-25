using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Musmetaniac.Common.Extensions;
using Musmetaniac.Web.Common.Models;

namespace Musmetaniac.Web.Client
{
    public class PollingJob<TResult> : IDisposable where TResult : class
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger? _logger;
        private readonly Func<HttpRequestMessage> _requestMessageFactory;
        private readonly TimeSpan _pollingPeriod;
        private readonly bool _stopOnFail;
        private readonly Action<TResult>? _successCallback;
        private readonly Action<string>? _failureCallback;
        private readonly Action? _finallyCallback;

        private CancellationTokenSource? _cancellationTokenSource;

        public bool IsStarted => _cancellationTokenSource is { IsCancellationRequested: false };

        public PollingJob(HttpClient httpClient, Func<HttpRequestMessage> requestMessageFactory, PollingJobOptions<TResult> options, ILogger? logger = null)
        {
            _httpClient = httpClient;
            _logger = logger;
            _requestMessageFactory = requestMessageFactory;
            _pollingPeriod = options.PollingPeriod ?? TimeSpan.FromSeconds(5);
            _stopOnFail = options.StopOnFail;
            _successCallback = options.SuccessCallback;
            _failureCallback = options.FailureCallback;
            _finallyCallback = options.FinallyCallback;
        }

        public void Run()
        {
            DisposeCancellationTokenSource();
            _cancellationTokenSource = new CancellationTokenSource();
            _ = RunPeriodicTimerAsync(_cancellationTokenSource.Token);
        }

        public void Stop()
        {
            DisposeCancellationTokenSource();
        }

        public void Dispose()
        {
            DisposeCancellationTokenSource();
            GC.SuppressFinalize(this);
        }

        private async Task RunPeriodicTimerAsync(CancellationToken cancellationToken)
        {
            try
            {
                using var periodicTimer = new PeriodicTimer(_pollingPeriod);
                do
                {
                    await DoPeriodicCallAsync(cancellationToken);
                } while (await periodicTimer.WaitForNextTickAsync(cancellationToken));
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "An error occurred while running the polling job.");
            }
        }

        private async Task DoPeriodicCallAsync(CancellationToken cancellationToken)
        {
            HttpResponseMessage response;
            try
            {
                response = await _httpClient.SendAsync(_requestMessageFactory(), cancellationToken);
            }
            catch (HttpRequestException)
            {
                HandleFailure("Network error.");
                _finallyCallback?.Invoke();

                return;
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode || content.IsNullOrEmpty())
                HandleFailure(content.FromJson<ErrorResult>()?.Message ?? "Unexpected server response format.");
            else
                _successCallback?.Invoke(content.FromJson<TResult>()!);

            _finallyCallback?.Invoke();

            void HandleFailure(string message)
            {
                if (_stopOnFail)
                    Stop();

                _failureCallback?.Invoke(message);
            }
        }

        private void DisposeCancellationTokenSource()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }
    }

    public class PollingJobOptions<TResult>
    {
        public Action<TResult>? SuccessCallback { get; set; }
        public Action<string>? FailureCallback { get; set; }
        public Action? FinallyCallback { get; set; }
        public TimeSpan? PollingPeriod { get; set; }
        public bool StopOnFail { get; set; }
    }
}
