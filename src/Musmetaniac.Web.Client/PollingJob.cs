using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Musmetaniac.Common.Extensions;
using Musmetaniac.Web.Common.Models;

namespace Musmetaniac.Web.Client
{
    public class PollingJob<TResult> : IAsyncDisposable where TResult : class
    {
        private readonly HttpClient _httpClient;
        private readonly Timer _timer;
        private readonly TimeSpan _pollingPeriod;
        private readonly TimeSpan _defaultPollingPeriod = TimeSpan.FromSeconds(5);
        private readonly CancellationTokenSource _cancellationTokenSource = new();

        public string Url { get; set; }
        public Action<TResult> SuccessCallback { get; set; }
        public Action<string> FailCallback { get; set; }
        public Action CompletionCallback { get; set; }
        public bool StopOnFail { get; set; }

        public bool IsStopped { get; private set; }

        public PollingJob(HttpClient httpClient, Options options)
        {
            _pollingPeriod = options.PollingPeriod ?? _defaultPollingPeriod;
            Url = options.Url;
            SuccessCallback = options.SuccessCallback;
            FailCallback = options.FailCallback;
            CompletionCallback = options.CompleteCallback;
            StopOnFail = options.StopOnFail;
            IsStopped = !options.StartImmediately;

            _httpClient = httpClient;
            _timer = new Timer(DoPeriodicCall, null, options.StartImmediately ? TimeSpan.Zero : Timeout.InfiniteTimeSpan, _pollingPeriod);
        }

        public void Restart()
        {
            IsStopped = false;
            _timer.Change(TimeSpan.Zero, _pollingPeriod);
        }

        public void Stop()
        {
            IsStopped = true;
            _timer.Change(Timeout.InfiniteTimeSpan, _pollingPeriod);
        }

        public async ValueTask DisposeAsync()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            await _timer.DisposeAsync();

            GC.SuppressFinalize(this);
        }

        private async void DoPeriodicCall(object state)
        {
            try
            {
                HttpResponseMessage response;
                try
                {
                    response = await _httpClient.GetAsync(Url, _cancellationTokenSource.Token);
                }
                catch (HttpRequestException)
                {
                    OnFail("Network error.");
                    CompletionCallback?.Invoke();

                    return;
                }

                var content = await response.Content.ReadAsStringAsync(_cancellationTokenSource.Token);

                if (response.IsSuccessStatusCode)
                    SuccessCallback?.Invoke(content.FromJson<TResult>());
                else
                    OnFail(content.FromJson<ErrorResult>().Message);

                CompletionCallback?.Invoke();
            }
            catch (TaskCanceledException) when (_cancellationTokenSource.Token.IsCancellationRequested)
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
            public Action<TResult> SuccessCallback { get; set; }
            public Action<string> FailCallback { get; set; }
            public Action CompleteCallback { get; set; }
            public TimeSpan? PollingPeriod { get; set; }
            public bool StopOnFail { get; set; }
            public bool StartImmediately { get; set; } = true;
        }
    }
}
