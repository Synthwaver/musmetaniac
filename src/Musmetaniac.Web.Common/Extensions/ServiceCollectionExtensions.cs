using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Musmetaniac.Web.Common.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureOptions<TOptions>(this IServiceCollection self, IConfiguration configuration) where TOptions : class, new()
        {
            self.AddSingleton<IOptionsChangeTokenSource<TOptions>>(new ConfigurationChangeTokenSource<TOptions>(string.Empty, configuration));
            self.AddSingleton<IConfigureOptions<TOptions>>(new NamedConfigureFromConfigurationOptions<TOptions>(string.Empty, configuration));
            self.AddSingleton(sp => sp.GetRequiredService<IOptions<TOptions>>().Value);

            return self;
        }
    }
}
