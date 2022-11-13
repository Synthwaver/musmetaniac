using System.Net.Http;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Musmetaniac.Web.Client;
using Musmetaniac.Web.Common.Extensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.ConfigureOptions<AppSettings>(builder.Configuration.GetSection("AppSettings"));

builder.Services.AddSingleton<HttpClient>();
builder.Services.AddSingleton<IRecurringJobFactory, RecurringJobFactory>();
builder.Services.AddSingleton<IMusmetaniacApiRequestMessageProvider, MusmetaniacApiRequestMessageProvider>();
builder.Services.AddAntDesign();

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

await builder.Build().RunAsync();
