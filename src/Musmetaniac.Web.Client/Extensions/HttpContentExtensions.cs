using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Musmetaniac.Common.Extensions;

namespace Musmetaniac.Web.Client.Extensions
{
    public static class HttpContentExtensions
    {
        public static async Task<TResult?> ReadJsonAsync<TResult>(this HttpContent self, CancellationToken? cancellationToken = null) where TResult : class
        {
            var content = await self.ReadAsStringAsync(cancellationToken ?? CancellationToken.None);

            return content.FromJson<TResult>();
        }
    }
}
