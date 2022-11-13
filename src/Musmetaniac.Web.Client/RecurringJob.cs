using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Musmetaniac.Web.Client
{
    public class RecurringJob : IDisposable
    {
        private readonly Func<CancellationToken, Task> _action;
        private readonly ILogger? _logger;
        private readonly TimeSpan _interval;

        private CancellationTokenSource? _cancellationTokenSource;

        public bool IsStarted => _cancellationTokenSource is { IsCancellationRequested: false };

        public RecurringJob(Func<CancellationToken, Task> action, TimeSpan? interval, ILogger? logger = null)
        {
            _action = action;
            _interval = interval ?? TimeSpan.FromSeconds(5);
            _logger = logger;
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
                using var periodicTimer = new PeriodicTimer(_interval);
                do
                {
                    await _action(cancellationToken);
                } while (await periodicTimer.WaitForNextTickAsync(cancellationToken));
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "An error occurred while running the recurring job.");
            }
        }

        private void DisposeCancellationTokenSource()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }
    }
}
