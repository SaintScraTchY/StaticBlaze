using System.Text;
using StaticBlaze.Constants;
using StaticBlaze.Models;
using StaticBlaze.Utilities.MarkdownParser;
using System.Text.Json;

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
        var docResponse = _httpClient.GetStringAsync($"{_navigationManager.BaseUri}Data/{StaticBlazeConfig.BlogDocs}/{slug}.md");
        var postResponse = _httpClient.GetStringAsync($"{_navigationManager.BaseUri}Data/{StaticBlazeConfig.BlogPosts}/{slug}.html");
        await Task.WhenAll(docResponse, postResponse);

        var postSummary = docResponse.Result;
        var postContent = postResponse.Result;
        if (string.IsNullOrEmpty(postSummary))
        {
            Console.WriteLine($"Post not found: {slug}");
            return null;
        }

        var post = postSummary.ParseMarkdown();
        post.Content = docResponse.Result.ToHtml();
        return post;
    }

    public ValueTask<IList<BlogPost>> GetPosts(int page, int pageSize, string? search = null, string? tag = null, string? category = null)
    {
        throw new NotImplementedException();
    }

    public async ValueTask<IList<BlogPost>> GetLandingPosts()
    {
        return new List<BlogPost>(
            new List<BlogPost>
            {
                new BlogPost
                {
                    Title = "Welcome to StaticBlaze",
                    Slug = "welcome-to-staticblaze",
                    Thumbnail = "https://picsum.photos/1280/720",
                    ShortDescription = "An introduction to StaticBlaze, a static site generator for Blazor.",
                    PublishedAt = DateTime.Now,
                    Author = "StaticBlaze Team",
                    Tags = "Blazor, Static Site Generator",
                    Category = "Introduction",
                    ReadTime = 3,
                    CreatedDateTime = DateTime.Now,
                    ModifiedDateTime = DateTime.Now,
                    Guid = Guid.NewGuid()
                },
                new BlogPost
                {
                    Title = "Getting Started with StaticBlaze",
                    Slug = "getting-started-with-staticblaze",
                    Thumbnail = "https://picsum.photos/1280/720",
                    ShortDescription = "A guide to setting up your first StaticBlaze project.",
                    PublishedAt = DateTime.Now.AddDays(-1),
                    Author = "StaticBlaze Team",
                    Tags = "Blazor, Getting Started",
                    Category = "Getting Started",
                    ReadTime = 5,
                    CreatedDateTime = DateTime.Now.AddDays(-1),
                    ModifiedDateTime = DateTime.Now.AddDays(-1),
                    Guid = Guid.NewGuid()
                },
                new BlogPost
                {
                    Title = "Advanced Features of StaticBlaze",
                    Slug = "advanced-features-of-staticblaze",
                    Thumbnail = "https://picsum.photos/1280/720",
                    ShortDescription = "Explore the advanced features of StaticBlaze for building static sites.",
                    PublishedAt = DateTime.Now.AddDays(-2),
                    Author = "StaticBlaze Team",
                    Tags = "Blazor, Advanced Features",
                    Category = "Advanced",
                    ReadTime = 7,
                    CreatedDateTime = DateTime.Now.AddDays(-2),
                    ModifiedDateTime = DateTime.Now.AddDays(-2),
                    Guid = Guid.NewGuid()
                },
                new BlogPost
                {
                    Title = "StaticBlaze and GitHub Pages",
                    Slug = "staticblaze-and-github-pages",
                    Thumbnail = "https://picsum.photos/1280/720",
                    ShortDescription = "Learn how to deploy your StaticBlaze site to GitHub Pages.",
                    PublishedAt = DateTime.Now.AddDays(-3),
                    Author = "StaticBlaze Team",
                    Tags = "Blazor, GitHub Pages",
                    Category = "Deployment",
                    ReadTime = 6,
                    CreatedDateTime = DateTime.Now.AddDays(-3),
                    ModifiedDateTime = DateTime.Now.AddDays(-3),
                    Guid = Guid.NewGuid()
                },
                new BlogPost
                {
                    Title = "Customizing StaticBlaze",
                    Slug = "customizing-staticblaze",
                    Thumbnail = "https://picsum.photos/1280/720",
                    ShortDescription = "A guide to customizing your StaticBlaze site with themes and plugins.",
                    PublishedAt = DateTime.Now.AddDays(-4),
                    Author = "StaticBlaze Team",
                    Tags = "Blazor, Customization",
                    Category = "Customization",
                    ReadTime = 4,
                    CreatedDateTime = DateTime.Now.AddDays(-4),
                    ModifiedDateTime = DateTime.Now.AddDays(-4),
                    Guid = Guid.NewGuid()
                },
                new BlogPost
                {
                    Title = "StaticBlaze Performance Tips",
                    Slug = "staticblaze-performance-tips",
                    Thumbnail = "https://picsum.photos/1280/720",
                    ShortDescription = "Tips for optimizing the performance of your StaticBlaze site.",
                    PublishedAt = DateTime.Now.AddDays(-5),
                    Author = "StaticBlaze Team",
                    Tags = "Blazor, Performance",
                    Category = "Performance",
                    ReadTime = 5,
                    CreatedDateTime = DateTime.Now.AddDays(-5),
                    ModifiedDateTime = DateTime.Now.AddDays(-5),
                    Guid = Guid.NewGuid()
                },
                new BlogPost
                {
                    Title = "StaticBlaze Community and Support",
                    Slug = "staticblaze-community-and-support",
                    Thumbnail = "https://picsum.photos/1280/720",
                    ShortDescription = "Join the StaticBlaze community and find support resources.",
                    PublishedAt = DateTime.Now.AddDays(-6),
                    Author = "StaticBlaze Team",
                    Tags = "Blazor, Community",
                    Category = "Community",
                    ReadTime = 3,
                    CreatedDateTime = DateTime.Now.AddDays(-6),
                    ModifiedDateTime = DateTime.Now.AddDays(-6),
                    Guid = Guid.NewGuid()
                },
                new BlogPost
                {
                    Title = "StaticBlaze Roadmap",
                    Slug = "staticblaze-roadmap",
                    Thumbnail = "https://picsum.photos/1280/720",
                    ShortDescription = "What's next for StaticBlaze? Check out our roadmap.",
                    PublishedAt = DateTime.Now.AddDays(-7),
                    Author = "StaticBlaze Team",
                    Tags = "Blazor, Roadmap",
                    Category = "Roadmap",
                    ReadTime = 4,
                    CreatedDateTime = DateTime.Now.AddDays(-7),
                    ModifiedDateTime = DateTime.Now.AddDays(-7),
                    Guid = Guid.NewGuid()
                }
            });
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
            new() { Title = "Blazor Best Practices", Slug = "blazor-best-practices", ShortDescription = "A guide to Blazor best practices.", PublishedAt = DateTime.Now.AddDays(-1), Author = "John Doe", Tags = "Blazor, C#", ReadTime = 5, CreatedDateTime = DateTime.Now, ModifiedDateTime = DateTime.Now, Guid = Guid.NewGuid() },
            new() { Title = "C# 11 Features", Slug = "csharp-11-features", ShortDescription = "Exploring the new features in C# 11.", PublishedAt = DateTime.Now.AddDays(-2), Author = "Jane Smith", Tags = "C#, .NET", ReadTime = 8, CreatedDateTime = DateTime.Now, ModifiedDateTime = DateTime.Now, Guid = Guid.NewGuid() },
            new() { Title = "ASP.NET Core Updates", Slug = "aspnet-core-updates", ShortDescription = "Latest updates in ASP.NET Core.", PublishedAt = DateTime.Now.AddDays(-3), Author = "Alice Johnson", Tags = "ASP.NET, Core", ReadTime = 10, CreatedDateTime = DateTime.Now, ModifiedDateTime = DateTime.Now, Guid = Guid.NewGuid() }
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

    public async Task<List<Author>> GetAllAuthorsAsync()
    {
        try
        {
            // var content = await _githubService.GetFileContentAsync(_authorsPath);
            //  return content != null 
            //      ? JsonSerializer.Deserialize<List<Author>>(content) ?? []
            //      : [];
            return [];
        }
        catch
        {
            return [];
        }
    }

    public async Task<Author?> GetAuthorByIdAsync(Guid id)
    {
        var authors = await GetAllAuthorsAsync();
        return authors.FirstOrDefault(a => a.Id == id);
    }

    public async Task<bool> CreateAuthorAsync(Author author)
    {
        try
        {
            var authors = await GetAllAuthorsAsync();
            authors.Add(author);
            var json = JsonSerializer.Serialize(authors, new JsonSerializerOptions { WriteIndented = true });
            await _githubService.UploadFileAsync(Encoding.UTF8.GetBytes(json),StaticBlazeConfig.BlogAuthors);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdateAuthorAsync(Author author)
    {
        try
        {
            var authors = await GetAllAuthorsAsync();
            var index = authors.FindIndex(a => a.Id == author.Id);
            if (index == -1) return false;
            
            authors[index] = author;
            author.LastUpdated = DateTime.UtcNow;
            
            var json = JsonSerializer.Serialize(authors, new JsonSerializerOptions { WriteIndented = true });
            await _githubService.UploadFileAsync(Encoding.UTF8.GetBytes(json),StaticBlazeConfig.BlogAuthors);
            return true;
        }
        catch
        {
            return false;
        }
    }
    public async Task<bool> DeleteAuthorAsync(Guid id)
    {
        try
        {
            var authors = await GetAllAuthorsAsync();
            var authorToRemove = authors.FirstOrDefault(a => a.Id == id);
            if (authorToRemove == null) return false;
            
            authors.Remove(authorToRemove);
            var json = JsonSerializer.Serialize(authors, new JsonSerializerOptions { WriteIndented = true });
            await _githubService.UploadFileAsync(Encoding.UTF8.GetBytes(json),StaticBlazeConfig.BlogAuthors);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<List<Tag>> GetTagsAsync()
    {
        try
        {
            var json = await GetFileContentAsync(StaticBlazeConfig.BlogTags);
            return string.IsNullOrEmpty(json) 
                ? []
                : JsonSerializer.Deserialize<List<Tag>>(json) ?? [];
            return
            [
                new Tag
                {
                    Id = Guid.NewGuid(), Title = "Blazor", Slug = "blazor", LastUpdated = DateTime.UtcNow,
                    Posts = []
                },
                new Tag
                {
                    Id = Guid.NewGuid(), Title = "Static Site Generator", Slug = "static-site-generator",
                    LastUpdated = DateTime.UtcNow, 
                    Posts = []
                },
                new Tag
                {
                    Id = Guid.NewGuid(), Title = "WebAssembly", Slug = "webassembly", LastUpdated = DateTime.UtcNow,
                    Posts = []
                }
            ];
        }
        catch (Exception ex)
        {
            return [];
        }
    }

    public async Task<Tag?> GetTagByIdAsync(Guid id)
    {
        var tags = await GetTagsAsync();
        return tags.FirstOrDefault(t => t.Id == id);
    }

    public async Task<bool> CreateTagAsync(Tag tag)
    {
        try
        {
            var tags = await GetTagsAsync();
            tags.Add(tag);
            var json = JsonSerializer.Serialize(tags, new JsonSerializerOptions { WriteIndented = true });
            await _githubService.UploadFileAsync(Encoding.UTF8.GetBytes(json),StaticBlazeConfig.BlogTags);
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public async Task<bool> UpdateTagAsync(Tag tag)
    {
        try
        {
            var tags = await GetTagsAsync();
            var index = tags.FindIndex(t => t.Id == tag.Id);
            if (index == -1) return false;

            tags[index] = tag;
            var json = JsonSerializer.Serialize(tags, new JsonSerializerOptions { WriteIndented = true });
            await _githubService.UploadFileAsync(Encoding.UTF8.GetBytes(json),StaticBlazeConfig.BlogTags);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteTagAsync(Guid id)
    {
        try
        {
            var tags = await GetTagsAsync();
            var tag = tags.FirstOrDefault(t => t.Id == id);
            if (tag == null) return false;

            tags.Remove(tag);
            var json = JsonSerializer.Serialize(tags, new JsonSerializerOptions { WriteIndented = true });
            await _githubService.UploadFileAsync(Encoding.UTF8.GetBytes(json),StaticBlazeConfig.BlogTags);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<List<Category>> GetCategoriesAsync()
    {
        try
        {
            var json = await GetFileContentAsync(StaticBlazeConfig.BlogCategories);
            return string.IsNullOrEmpty(json) 
                ? []
                : JsonSerializer.Deserialize<List<Category>>(json) ?? [];
            return [];
        }
        catch
        {
            return [];
        }
    }

    public async Task<Category?> GetCategoryByIdAsync(Guid id)
    {
        var categories = await GetCategoriesAsync();
        return categories.FirstOrDefault(c => c.Id == id);
    }

    public async Task<string?> GetFileContentAsync(string filePath)
    {
        var response = await _httpClient.GetStringAsync($"{_navigationManager.BaseUri}Data/{filePath}");
        return response;
    }

    public async Task<bool> CreateCategoryAsync(Category category)
    {
        try
        {
            var categories = await GetCategoriesAsync();
            categories.Add(category);
            var json = JsonSerializer.Serialize(categories, new JsonSerializerOptions { WriteIndented = true });
            await _githubService.UploadFileAsync(Encoding.UTF8.GetBytes(json),StaticBlazeConfig.BlogCategories);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdateCategoryAsync(Category category)
    {
        try
        {
            var categories = await GetCategoriesAsync();
            var index = categories.FindIndex(c => c.Id == category.Id);
            if (index == -1) return false;

            categories[index] = category;
            var json = JsonSerializer.Serialize(categories, new JsonSerializerOptions { WriteIndented = true });
            await _githubService.UploadFileAsync(Encoding.UTF8.GetBytes(json),StaticBlazeConfig.BlogCategories);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteCategoryAsync(Guid id)
    {
        try
        {
            var categories = await GetCategoriesAsync();
            var category = categories.FirstOrDefault(c => c.Id == id);
            if (category == null) return false;

            categories.Remove(category);
            var json = JsonSerializer.Serialize(categories, new JsonSerializerOptions { WriteIndented = true });
            await _githubService.UploadFileAsync(Encoding.UTF8.GetBytes(json),StaticBlazeConfig.BlogCategories);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
