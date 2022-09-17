using System;
using System.Reflection;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Musmetaniac.Services;
using Musmetaniac.Web.Serverless;
using Musmetaniac.Web.Serverless.Extensions;

[assembly: FunctionsStartup(typeof(Startup))]
namespace Musmetaniac.Web.Serverless
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.Configure<ServiceAppSettings>("AppSettings");
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
