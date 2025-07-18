using StaticBlaze.Models;
using StaticBlaze.Services;

namespace StaticBlaze.Components.Pages.Client;

public partial class Home : ComponentBase
{
    private readonly IBlogService _blogService;

    public Home(IBlogService blogService)
    {
        _blogService = blogService;
    }

    private bool Loading { get; set; } = true;
    private List<BlogPost> FeaturedPosts { get; set; } = new();
    private List<BlogPost> LatestPosts { get; set; } = new();
    private Dictionary<string, int> Categories { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var allPosts = await _blogService.GetLandingPosts();
            
            Console.WriteLine($"has Any Posts: {allPosts.Any()} - Count: {allPosts?.Count ?? 0}");
            
            FeaturedPosts = allPosts.Where(p => p.Featured).Take(2).ToList();
            LatestPosts = allPosts.OrderByDescending(p => p.Guid).Take(6).ToList();
            Categories = allPosts
                .GroupBy(p => p.Category)
                .ToDictionary(g => g.Key, g => g.Count());
        }
        finally
        {
            Loading = false;
        }
    }
}