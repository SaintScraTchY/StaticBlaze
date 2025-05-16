using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using StaticBlazeWASM.Constants;
using StaticBlazeWASM.Models;
using StaticBlazeWASM.Utilities;

namespace StaticBlazeWASM.Services;

public class GithubService
{
    private readonly HttpClient Http;
    private readonly ILocalStorageService LocalStorage;

    public GithubService(HttpClient http, ILocalStorageService localStorage)
    {
        Http = http;
        LocalStorage = localStorage;
    }

    public async Task<bool> ProcessMarkDown(BlogPost metaPost, string message,string fileName)
    {
        var content = metaPost.GenerateMarkdownWithMetadata();
        if (!await UploadMarkDown(content, message, fileName)) return false;
        
        var htmlContent = MarkdownHelper.ToHtml(content);
        return await UploadMarkdownHtmlPage(htmlContent, message, fileName);

    }

    private async Task<bool> UploadMarkDown(string content, string message,string fileName)
    {
        var ghPAT = await LocalStorage.GetItemAsStringAsync("GitHubToken");
        if (string.IsNullOrEmpty(ghPAT)) return false;
        
        var githubApiUrl = $"https://api.github.com/repos/{GithubConfig.Owner}/{GithubConfig.Repo}/contents/{StaticBlazeConfig.ProjectName}{StaticBlazeConfig.BlogDocs}/{fileName}.md";

        var request = new HttpRequestMessage(HttpMethod.Put, githubApiUrl)
        {
            Content = JsonContent.Create(new
            {
                message,
                content = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(content)),
                branch = GithubConfig.Branch
            })
        };
        request.Headers.UserAgent.ParseAdd("StaticBlazeWASM");
        request.Headers.Authorization = new AuthenticationHeaderValue("token",ghPAT);

        var response = await Http.SendAsync(request);
        return response.IsSuccessStatusCode;
    }
    
    private async Task<bool> UploadMarkdownHtmlPage(string content, string message,string fileName)
    {
        var ghPAT = await LocalStorage.GetItemAsStringAsync("GitHubToken");
        if (string.IsNullOrEmpty(ghPAT)) return false;
        
        var githubApiUrl = $"https://api.github.com/repos/{GithubConfig.Owner}/{GithubConfig.Repo}/contents/{StaticBlazeConfig.ProjectName}{StaticBlazeConfig.BlogPosts}/{fileName}.html";

        var request = new HttpRequestMessage(HttpMethod.Put, githubApiUrl)
        {
            Content = JsonContent.Create(new
            {
                message,
                content = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(content)),
                branch = GithubConfig.Branch
            })
        };
        request.Headers.UserAgent.ParseAdd("StaticBlazeWASM");
        request.Headers.Authorization = new AuthenticationHeaderValue("token",ghPAT);

        var response = await Http.SendAsync(request);
        return response.IsSuccessStatusCode;
    }
    
    public async Task<string> UploadImageToGitHub(byte[] imageBytes, string fileName)
    {
        var ghPAT = await LocalStorage.GetItemAsStringAsync("GitHubToken");
        if (string.IsNullOrEmpty(ghPAT)) return null;
        
        var path = $"{StaticBlazeConfig.ProjectName}{StaticBlazeConfig.ProjectName}{StaticBlazeConfig.BlogAssets}/{fileName}"; // Path inside the repository
        var githubApiUrl = $"https://api.github.com/repos/{GithubConfig.Owner}/{GithubConfig.Repo}/contents/{path}";

        var content = new
        {
            message = $"Upload image {fileName}",
            content = Convert.ToBase64String(imageBytes),
            branch = GithubConfig.Branch
        };

        var request = new HttpRequestMessage(HttpMethod.Put, githubApiUrl)
        {
            Content = JsonContent.Create(content)
        };
        request.Headers.UserAgent.ParseAdd("StaticBlazeWASM");
        request.Headers.Authorization = new AuthenticationHeaderValue("token", ghPAT);

        var response = await Http.SendAsync(request);
        if (!response.IsSuccessStatusCode) return string.Empty;
        
        var responseData = await response.Content.ReadFromJsonAsync<GitHubUploadResponse>();
        return responseData?.Content?.DownloadUrl ?? string.Empty;
    }
}