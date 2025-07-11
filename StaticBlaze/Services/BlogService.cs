using StaticBlaze.Constants;
using StaticBlaze.Models;
using StaticBlaze.Utilities.MarkdownParser;

namespace StaticBlaze.Services;

public class BlogService : IBlogService
{
    private readonly HttpClient _httpClient;
    private readonly IGithubService _githubService;
    private readonly IAnalyticsService _analyticsService;
    private readonly NavigationManager _navigationManager;

    public BlogService(HttpClient httpClient, NavigationManager navigationManager, IGithubService githubService, IAnalyticsService analyticsService)
    {
        _httpClient = httpClient;
        _navigationManager = navigationManager;
        _githubService = githubService;
        _analyticsService = analyticsService;
    }

    public async Task<BlogPost?> GetPostAsync(string slug)
    {
        var docuri = $"{_navigationManager.BaseUri}{StaticBlazeConfig.BlogDocs}/{slug}.md";
        var docResponse = _httpClient.GetStringAsync($"{_navigationManager.BaseUri}{StaticBlazeConfig.BlogDocs}/{slug}.md.gz");
        var postResponse = _httpClient.GetStringAsync($"{_navigationManager.BaseUri}{StaticBlazeConfig.BlogPosts}/{slug}.html.gz");
        await Task.WhenAll(docResponse, postResponse);

        var postSummary = docResponse.Result;
        var postContent = postResponse.Result;
        if (string.IsNullOrEmpty(postSummary) || string.IsNullOrEmpty(postContent))
        {
            return null;
        }

        var post = postSummary.ParseMarkdown();
        post.Content = docResponse.Result;
        return post;
    }
    
    public async Task<DashboardStats> GetDashboardStats()
    {
        var currentPosts = await _githubService.GetTotalPosts();
        var lastCommitDate = await _githubService.GetLastCommitDate();
        var (views, activeUsers) = await _analyticsService.GetAnalyticsData();
        var averageLoadTime = await _analyticsService.GetAverageLoadTime();

        // Calculate growth percentages
        // This is a simplified calculation - you might want to implement more sophisticated logic
        var stats = new DashboardStats
        {
            TotalPosts = currentPosts,
            TotalViews = views,
            ActiveUsers = activeUsers,
            TotalComments = 0, // GitHub doesn't provide this directly - consider using GitHub Discussions or a separate comment system
            
            // Example growth calculations (you should implement proper historical data comparison)
            PostsGrowth = 5.0,
            ViewsGrowth = 10.0,
            UsersGrowth = 15.0,
            CommentsGrowth = 0,
            
            // Performance metrics
            AverageLoadTime = (int)(averageLoadTime * 1000), // Convert to milliseconds
            LoadTimePercentage = 85.0,
            ServerResponseTime = 200,
            ServerResponsePercentage = 95.0,
            UptimePercentage = 99.9
        };

        return stats;
    }

    public async Task<List<MetaPost>> GetPostsAsync()
    {
        var response = await _httpClient.GetAsync($"/posts");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<MetaPost>>();
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
