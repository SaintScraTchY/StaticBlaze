using System.Net.Http.Headers;
using System.Text.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using StaticBlaze.Constants;
using StaticBlaze.Models;

namespace StaticBlaze.Components.Pages.Client;

public partial class Login : ComponentBase
{
    private readonly NavigationManager _navigation;
    private readonly ILocalStorageService _localStorage;
    private string GitHubToken;

    public Login(NavigationManager navigation, ILocalStorageService localStorage)
    {
        _navigation = navigation;
        _localStorage = localStorage;
    }

    private async Task HandleLogin()
    {
        if (string.IsNullOrEmpty(GitHubToken)) return;

        // Call GitHub API to validate token
        var (isValid, username) = await VerifyGitHubToken(GitHubToken);
        if (isValid && username == GithubConfig.Owner) // Optionally verify username
        {
            var options = new CookieOptions { Expires = DateTime.UtcNow.AddDays(1), HttpOnly = true, Secure = true };
            await _localStorage.SetItemAsStringAsync("GitHubToken", GitHubToken);
            _navigation.NavigateTo("/admin/dashboard");
        }
        else
        {
            // Handle invalid token
        }
    }
    
    private async Task<(bool IsValid, string Username)> VerifyGitHubToken(string token)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        client.DefaultRequestHeaders.UserAgent.ParseAdd("BlazorApp");

        try
        {
            // Step 1: Get user info
            var userResponse = await client.GetAsync("https://api.github.com/user");
            if (!userResponse.IsSuccessStatusCode) return (false, null);

            var userJson = await userResponse.Content.ReadAsStringAsync();
            var user = JsonSerializer.Deserialize<GitHubUser>(userJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        
            // Step 2: Check if token has access to a specific repo (replace these)
            var repoCheck = await client.GetAsync($"https://api.github.com/repos/{GithubConfig.Owner}/{GithubConfig.Owner}");
            if (!repoCheck.IsSuccessStatusCode) return (false, null);

            return (true, user.Login);
        }
        catch
        {
            return (false, null);
        }
    }
}