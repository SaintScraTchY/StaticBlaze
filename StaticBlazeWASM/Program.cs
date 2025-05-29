using System.Net.Http;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using StaticBlazeWASM.Components;
using StaticBlazeWASM.Services;
using StaticBlazeWASM.Utilities;

var builder = WebAssemblyHostBuilder.CreateDefault(args);


builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<IGithubService,GithubService>();
builder.Services.AddScoped<IAnalyticsService,AnalyticsService>();
builder.Services.AddScoped<IBlogService,BlogService>();

builder.Services.AddBlazoredLocalStorage();
builder.SetGitConfig();

builder.Services.AddScoped(sp => new HttpClient { });

await builder.Build().RunAsync();