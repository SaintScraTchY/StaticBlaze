using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using StaticBlazeWASM.Components;
using StaticBlazeWASM.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);


builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<GithubService>();

builder.Services.AddScoped(sp => new HttpClient { });

await builder.Build().RunAsync();