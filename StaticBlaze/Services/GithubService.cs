using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Blazored.LocalStorage;
using StaticBlaze.Constants;
using StaticBlaze.Models;
using StaticBlaze.Utilities.MarkdownParser;

namespace StaticBlaze.Services;

public class GithubService : IGithubService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;

    public GithubService(HttpClient httpClient, ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
    }

    public async Task<bool> ProcessMarkDown(BlogPost metaPost, string message, string fileName)
    {
        var content = metaPost.GenerateMarkdownWithMetadata();
        if (!await UploadMarkDown(content, message, fileName)) return false;

        var htmlContent = MarkdownHelper.ToHtml(content);
        return await UploadMarkdownHtmlPage(htmlContent, message, fileName);
    }

    private async Task<bool> UploadMarkDown(string content, string message, string fileName)
    {
        var ghPAT = await _localStorage.GetItemAsStringAsync("GitHubToken");
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

        request.Headers.Authorization = new AuthenticationHeaderValue("token", ghPAT);
        request.Headers.UserAgent.ParseAdd("StaticBlazeWASM");

        var response = await _httpClient.SendAsync(request);
        return response.IsSuccessStatusCode;
    }

    private async Task<bool> UploadMarkdownHtmlPage(string content, string message, string fileName)
    {
        var ghPAT = await _localStorage.GetItemAsStringAsync("GitHubToken");
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

        request.Headers.Authorization = new AuthenticationHeaderValue("token", ghPAT);
        request.Headers.UserAgent.ParseAdd("StaticBlazeWASM");

        var response = await _httpClient.SendAsync(request);
        return response.IsSuccessStatusCode;
    }

    public async Task<string> UploadImageToGitHub(byte[] imageBytes, string fileName)
    {
        var ghPAT = await _localStorage.GetItemAsStringAsync("GitHubToken");
        if (string.IsNullOrEmpty(ghPAT)) return null;

        var path = $"{StaticBlazeConfig.ProjectName}{StaticBlazeConfig.BlogAssets}/{fileName}";
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

        request.Headers.Authorization = new AuthenticationHeaderValue("token", ghPAT);
        request.Headers.UserAgent.ParseAdd("StaticBlazeWASM");

        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode) return string.Empty;

        var responseData = await response.Content.ReadFromJsonAsync<GitHubUploadResponse>();
        return responseData?.DownloadUrl?.DownloadUrl ?? string.Empty;
    }

    public async Task<int> GetTotalPosts()
    {
        var ghPAT = await _localStorage.GetItemAsStringAsync("GitHubToken");
        if (string.IsNullOrEmpty(ghPAT)) return 0;

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", ghPAT);
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("StaticBlaze");

        var response = await _httpClient.GetAsync($"https://api.github.com/repos/{GithubConfig.Owner}/{GithubConfig.Repo}/{StaticBlazeConfig.ProjectName}{StaticBlazeConfig.BlogAssets}");
        var files = await response.Content.ReadFromJsonAsync<List<GitHubContentFileName>>();
        return files?.Count ?? 0;
    }

    public async Task<DateTime?> GetLastCommitDate()
    {
        var ghPAT = await _localStorage.GetItemAsStringAsync("GitHubToken");
        if (string.IsNullOrEmpty(ghPAT)) return null;

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ghPAT);
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("StaticBlaze");

        var response = await _httpClient.GetAsync($"https://api.github.com/repos/{GithubConfig.Owner}/{GithubConfig.Repo}/commits?path=_posts&per_page=1");
        var commits = await response.Content.ReadFromJsonAsync<List<GitHubCommit>>();
        return commits?.FirstOrDefault()?.commit.author.date;
    }

    private record GitHubContentFileName
    {
        public string name { get; set; } = string.Empty;
    }

    private class GitHubCommit
    {
        public CommitInfo commit { get; set; } = new();
        public class CommitInfo
        {
            public AuthorInfo author { get; set; } = new();
        }
        public class AuthorInfo
        {
            public DateTime date { get; set; }
        }
    }
}
