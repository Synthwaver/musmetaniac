using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Musmetaniac.Web.Client
{
    public interface IRecurringJobFactory
    {
        RecurringJob CreateRecurringJob(Func<CancellationToken, Task> action, TimeSpan? interval);
    }

    public class RecurringJobFactory : IRecurringJobFactory
    {
        private readonly ILoggerFactory _loggerFactory;

        public RecurringJobFactory(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public RecurringJob CreateRecurringJob(Func<CancellationToken, Task> action, TimeSpan? interval)
        {
            return new RecurringJob(action, interval, _loggerFactory.CreateLogger<RecurringJob>());
        }
    }
}
