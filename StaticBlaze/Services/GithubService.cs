using System.Net.Http.Headers;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Forms;
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
        request.Headers.UserAgent.ParseAdd("StaticBlaze");

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
        request.Headers.UserAgent.ParseAdd("StaticBlaze");

        var response = await _httpClient.SendAsync(request);
        return response.IsSuccessStatusCode;
    }

    public async Task<string> UploadImageToGitHub(byte[] imageBytes, string fileName)
    {
        return await UploadToGitHubAsync(imageBytes, fileName);
    }

    public async Task<string> UploadFileAsync(IBrowserFile file)
    {
        using var stream = file.OpenReadStream(maxAllowedSize: 10485760); // 10MB max
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);
        return await UploadToGitHubAsync(ms.ToArray(), file.Name);
    }

    public async Task<string> UploadFileAsync(Stream stream, string fileName)
    {
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);
        return await UploadToGitHubAsync(ms.ToArray(), fileName);
    }

    public async Task<string> UploadFileAsync(string base64Content, string fileName)
    {
        var bytes = Convert.FromBase64String(base64Content);
        return await UploadToGitHubAsync(bytes, fileName);
    }
    
    public async Task<string> UploadFileAsync(byte[] content, string fileName)
    {
        return await UploadToGitHubAsync(content, fileName);
    }

    public async Task<List<string>> UploadFilesAsync(IEnumerable<IBrowserFile> files)
    {
        var tasks = files.Select(async file =>
        {
            using var stream = file.OpenReadStream(maxAllowedSize: 10485760); // 10MB max
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            return await UploadToGitHubAsync(ms.ToArray(), file.Name);
        });

        var results = await Task.WhenAll(tasks);
        return results.Where(url => !string.IsNullOrEmpty(url)).ToList();
    }

    private async Task<string> UploadToGitHubAsync(byte[] fileBytes, string fileName)
    {
        var ghPAT = await _localStorage.GetItemAsStringAsync("GitHubToken");
        if (string.IsNullOrEmpty(ghPAT)) return string.Empty;

        //var path = $"{StaticBlazeConfig.ProjectName}{StaticBlazeConfig.BlogAssets}/{fileName}";
        var path = $"{StaticBlazeConfig.ProjectName}Data/{fileName}";
        var githubApiUrl = $"https://api.github.com/repos/{GithubConfig.Owner}/{GithubConfig.Repo}/contents/{path}";

        var content = new
        {
            message = $"Upload file {fileName}",
            content = Convert.ToBase64String(fileBytes),
            branch = GithubConfig.Branch
        };

        var request = new HttpRequestMessage(HttpMethod.Put, githubApiUrl)
        {
            Content = JsonContent.Create(content)
        };

        request.Headers.Authorization = new AuthenticationHeaderValue("token", ghPAT);
        request.Headers.UserAgent.ParseAdd("StaticBlaze");

        try
        {
            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) return string.Empty;

            var responseData = await response.Content.ReadFromJsonAsync<GitHubUploadResponse>();
            return responseData?.DownloadUrl?.DownloadUrl ?? string.Empty;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error uploading file: {ex.Message}");
            return string.Empty;
        }
    }

    public async Task<int> GetTotalPosts()
    {
        var ghPAT = await _localStorage.GetItemAsStringAsync("GitHubToken");
        if (string.IsNullOrEmpty(ghPAT)) return 0;

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token", ghPAT);
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("StaticBlaze");

        var githubApiUrl = $"https://api.github.com/repos/{GithubConfig.Owner}/{GithubConfig.Repo}/contents/{StaticBlazeConfig.ProjectName}Data/{StaticBlazeConfig.BlogDocs}?ref={GithubConfig.Branch}";
        Console.WriteLine(githubApiUrl);
        var response = await _httpClient.GetAsync(githubApiUrl);
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
