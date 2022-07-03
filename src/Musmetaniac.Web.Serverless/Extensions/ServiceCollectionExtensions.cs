using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Musmetaniac.Web.Serverless.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static OptionsBuilder<TOptions> Configure<TOptions>(this IServiceCollection self, string key) where TOptions : class
        {
            return self.AddOptions<TOptions>().Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.GetSection(key).Bind(settings);
            });
        }
    }
}
