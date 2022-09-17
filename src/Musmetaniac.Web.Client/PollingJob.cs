using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Musmetaniac.Common.Extensions;
using Musmetaniac.Web.Common.Models;

namespace Musmetaniac.Web.Client
{
    public class PollingJob<TResult> : IDisposable where TResult : class
    {
        private readonly HttpClient _httpClient;
        private readonly TimeSpan _pollingPeriod;
        private readonly TimeSpan _defaultPollingPeriod = TimeSpan.FromSeconds(5);

        private CancellationTokenSource? _cancellationTokenSource;

        public string Url { get; set; }
        public Action<TResult>? SuccessCallback { get; set; }
        public Action<string>? FailCallback { get; set; }
        public Action? CompletionCallback { get; set; }
        public bool StopOnFail { get; set; }

        public bool IsStopped => _cancellationTokenSource == null || _cancellationTokenSource.IsCancellationRequested;

        public PollingJob(HttpClient httpClient, Options options)
        {
            _httpClient = httpClient;
            _pollingPeriod = options.PollingPeriod ?? _defaultPollingPeriod;
            Url = options.Url;
            SuccessCallback = options.SuccessCallback;
            FailCallback = options.FailCallback;
            CompletionCallback = options.CompletionCallback;
            StopOnFail = options.StopOnFail;

            if (options.StartImmediately)
                Restart();
        }

        public void Restart()
        {
            if (!IsStopped)
                Stop();

            _cancellationTokenSource ??= new CancellationTokenSource();
            _ = RunPeriodicTimerAsync(_cancellationTokenSource.Token);
        }

        public void Stop()
        {
            if (IsStopped)
                return;

            _cancellationTokenSource!.Cancel();
            _cancellationTokenSource!.Dispose();
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();

            GC.SuppressFinalize(this);
        }

        private async Task RunPeriodicTimerAsync(CancellationToken cancellationToken)
        {
            using var periodicTimer = new PeriodicTimer(_pollingPeriod);
            do
            {
                await DoPeriodicCallAsync(cancellationToken);
            } while (await periodicTimer.WaitForNextTickAsync(cancellationToken));
        }

        private async Task DoPeriodicCallAsync(CancellationToken cancellationToken)
        {
            try
            {
                HttpResponseMessage response;
                try
                {
                    response = await _httpClient.GetAsync(Url, cancellationToken);
                }
                catch (HttpRequestException)
                {
                    OnFail("Network error.");
                    CompletionCallback?.Invoke();

                    return;
                }

                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (!response.IsSuccessStatusCode || content.IsNullOrEmpty())
                    OnFail(content.FromJson<ErrorResult>()?.Message ?? "Unexpected server response format.");
                else
                    SuccessCallback?.Invoke(content.FromJson<TResult>()!);

                CompletionCallback?.Invoke();
            }
            catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
            {
            }
        }

        private void OnFail(string message)
        {
            if (StopOnFail)
                Stop();

            FailCallback?.Invoke(message);
        }

        public class Options
        {
            public string Url { get; set; }
            public Action<TResult>? SuccessCallback { get; set; }
            public Action<string>? FailCallback { get; set; }
            public Action? CompletionCallback { get; set; }
            public TimeSpan? PollingPeriod { get; set; }
            public bool StopOnFail { get; set; }
            public bool StartImmediately { get; set; } = true;

            public Options(string url)
            {
                Url = url;
            }
        }
    }
}
