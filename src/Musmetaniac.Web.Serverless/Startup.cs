using System.Reflection;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Musmetaniac.Services;
using Musmetaniac.Web.Common.Extensions;
using Musmetaniac.Web.Serverless;

[assembly: FunctionsStartup(typeof(Startup))]
namespace Musmetaniac.Web.Serverless
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var configuration = builder.GetContext().Configuration;
            builder.Services.ConfigureOptions<ServiceAppSettings>(configuration.GetSection("AppSettings"));

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSingleton<IFunctionFilter, HandleFunctionExceptionFilter>();
            builder.Services.AddSingleton<IRecentTracksService, RecentTracksService>();
        }

        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            builder.ConfigurationBuilder.SetBasePath(builder.GetContext().ApplicationRootPath)
                .AddJsonFile("settings.json")
                .AddJsonFile("local.settings.json", optional: true)
                .AddUserSecrets(Assembly.GetExecutingAssembly(), optional: true)
                .AddEnvironmentVariables()
                .Build();
        }
    }
}
