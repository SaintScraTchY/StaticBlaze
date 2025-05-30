using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using StaticBlaze.Constants;

namespace StaticBlaze.Utilities;

public static class BlazorBuilder
{
    public static WebAssemblyHostBuilder SetGitConfig(this WebAssemblyHostBuilder builder)
    {
        var owner = builder.Configuration["GithubConfig:Owner"];
        var repo = builder.Configuration["GithubConfig:Repo"];
        var branch = builder.Configuration["GithubConfig:Branch"];
        var username = builder.Configuration["GithubConfig:Username"];
        GithubConfig.SetConfig(owner, repo, branch,username);
        return builder;
    }
    
}