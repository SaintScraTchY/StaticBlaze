using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using StaticBlazeWASM.Constants;
using StaticBlazeWASM.Models;
using StaticBlazeWASM.Utilities;
using System.ComponentModel.DataAnnotations;

namespace StaticBlazeWASM.Services;

public class BlogService : IBlogService
{
    private readonly HttpClient _httpClient;
    private readonly NavigationManager _navigationManager;

    public BlogService(HttpClient httpClient, NavigationManager navigationManager)
    {
        _httpClient = httpClient;
        _navigationManager = navigationManager;
    }

    public async Task<DashboardStats> GetDashboardStats()
    {
        
        // TODO: Replace with actual API call
        // Simulated data for demonstration
        return new DashboardStats
        {
            TotalPosts = 42,
            TotalViews = 12500,
            ActiveUsers = 156,
            TotalComments = 284,
            
            PostsGrowth = 12.5,
            ViewsGrowth = 25.8,
            UsersGrowth = -5.2,
            CommentsGrowth = 8.7,
            
            AverageLoadTime = 250,
            LoadTimePercentage = 85,
            ServerResponseTime = 120,
            ServerResponsePercentage = 92,
            UptimePercentage = 99.9
        };
    }

    public async Task<List<MetaPost>> GetPostsAsync()
    {
        var response = await _httpClient.GetAsync($"/posts");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<MetaPost>>();
    }

    public async Task<BlogPost?> GetPostAsync(string slug)
    {
        var docuri = $"{_navigationManager.BaseUri}{StaticBlazeConfig.BlogDocs}/{slug}.md";
        var docResponse = _httpClient.GetStringAsync($"{_navigationManager.BaseUri}{StaticBlazeConfig.BlogDocs}/{slug}.md");
        var postResponse = _httpClient.GetStringAsync($"{_navigationManager.BaseUri}{StaticBlazeConfig.BlogPosts}/{slug}.html");
        await Task.WhenAll(docResponse, postResponse);

        var postSummary = docResponse.Result;
        var postContent = postResponse.Result;
        if (string.IsNullOrEmpty(postSummary) || string.IsNullOrEmpty(postContent))
        {
            return null;
        }

        var post = MarkdownHelper.ParseMarkdown(postSummary);
        post.Content = postContent;
        return post;
    }

    public Task<List<MetaPost>> GetRecentPosts(int count)
    {
        // Dummy data for recent posts
        var recentPosts = new List<MetaPost>
        {
            new() { Title = "Blazor Best Practices", Slug = "blazor-best-practices", ShortDescription = "A guide to Blazor best practices.", PublishedAt = DateTime.Now.AddDays(-1), Author = "John Doe", Tags = "Blazor, C#", ReadTime = 5, CreatedDateTime = DateTime.Now, EditedDateTime = DateTime.Now, Guid = Guid.NewGuid() },
            new() { Title = "C# 11 Features", Slug = "csharp-11-features", ShortDescription = "Exploring the new features in C# 11.", PublishedAt = DateTime.Now.AddDays(-2), Author = "Jane Smith", Tags = "C#, .NET", ReadTime = 8, CreatedDateTime = DateTime.Now, EditedDateTime = DateTime.Now, Guid = Guid.NewGuid() },
            new() { Title = "ASP.NET Core Updates", Slug = "aspnet-core-updates", ShortDescription = "Latest updates in ASP.NET Core.", PublishedAt = DateTime.Now.AddDays(-3), Author = "Alice Johnson", Tags = "ASP.NET, Core", ReadTime = 10, CreatedDateTime = DateTime.Now, EditedDateTime = DateTime.Now, Guid = Guid.NewGuid() }
        };

        // Limit the number of posts to the requested count
        return Task.FromResult(recentPosts.Take(count).ToList());
    }

    public async Task<List<ActivityItem>> GetRecentActivity(int count)
    {
        return new List<ActivityItem>
        {
            new() {
                Description = "New comment on 'Getting Started with Blazor WebAssembly'",
                Timestamp = DateTime.Now.AddMinutes(-30),
                Type = ActivityType.Comment
            },
            new() {
                Description = "Post 'Building Modern UI with Tailwind CSS' published",
                Timestamp = DateTime.Now.AddHours(-2),
                Type = ActivityType.Post
            },
            new() {
                Description = "Media library updated with 5 new images",
                Timestamp = DateTime.Now.AddHours(-4),
                Type = ActivityType.Media
            },
            new() {
                Description = "New tag 'WebAssembly' created",
                Timestamp = DateTime.Now.AddHours(-6),
                Type = ActivityType.Tag
            }
        };
    }

    public Task<bool> DeletePost(string id)
    {
        throw new NotImplementedException();
    }
}
