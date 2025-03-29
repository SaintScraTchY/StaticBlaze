using System.Net.Http.Headers;
using System.Net.Http.Json;
using StaticBlazeWASM.Constants;
using StaticBlazeWASM.Models;
using StaticBlazeWASM.Utilities;

namespace StaticBlazeWASM.Services;

public class GithubService
{
    private readonly HttpClient Http;

    public GithubService(HttpClient http)
    {
        Http = http;
    }

    public async Task<bool> ProcessMarkDown(string content, string message,string fileName)
    {
        if (!await UploadMarkDown(content, message, fileName)) return false;
        
        var htmlContent = MarkdownHelper.ToHtml(content);
        return await UploadMarkdownHtmlPage(content, message, fileName);

    }
    
    private async Task<bool> UploadMarkDown(string content, string message,string fileName)
    {
        var githubApiUrl = $"https://api.github.com/repos/{GithubConfig.Owner}/{GithubConfig.Repo}/contents/Posts/{fileName}.md";

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
        request.Headers.Authorization = new AuthenticationHeaderValue("token", "your-github-token");

        var response = await Http.SendAsync(request);
        return response.IsSuccessStatusCode;
    }
    
    private async Task<bool> UploadMarkdownHtmlPage(string content, string message,string fileName)
    {
        var githubApiUrl = $"https://api.github.com/repos/{GithubConfig.Owner}/{GithubConfig.Repo}/contents/Pages/{fileName}.html";

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
        request.Headers.Authorization = new AuthenticationHeaderValue("token", "your-github-token");

        var response = await Http.SendAsync(request);
        return response.IsSuccessStatusCode;
    }
    
    public async Task<string> UploadImageToGitHub(byte[] imageBytes, string fileName)
    {
        var path = $"Images/{fileName}"; // Path inside the repository
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
        request.Headers.Authorization = new AuthenticationHeaderValue("token", "your-github-token");

        var response = await Http.SendAsync(request);
        if (!response.IsSuccessStatusCode) return string.Empty;
        
        var responseData = await response.Content.ReadFromJsonAsync<GitHubUploadResponse>();
        return responseData?.Content?.DownloadUrl ?? string.Empty;
    }
}