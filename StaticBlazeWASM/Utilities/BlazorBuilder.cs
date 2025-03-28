using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using StaticBlazeWASM.Constants;

namespace StaticBlazeWASM.Utilities;

public static class BlazorBuilder
{
    public static WebAssemblyHostBuilder SetGitConfig(this WebAssemblyHostBuilder builder)
    {
        var owner = builder.Configuration["GithubConfig:Owner"];
        var repo = builder.Configuration["GithubConfig:Repo"];
        var branch = builder.Configuration["GithubConfig:Branch"];
        GithubConfig.SetConfig(owner, repo, branch);
        return builder;
    }
    
}