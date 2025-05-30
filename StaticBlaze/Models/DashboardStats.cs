namespace StaticBlaze.Models;

public class DashboardStats
{
    public int TotalPosts { get; set; }
    public int TotalViews { get; set; }
    public int ActiveUsers { get; set; }
    public int TotalComments { get; set; }
    
    public double PostsGrowth { get; set; }
    public double ViewsGrowth { get; set; }
    public double UsersGrowth { get; set; }
    public double CommentsGrowth { get; set; }
    
    // Performance metrics
    public int AverageLoadTime { get; set; }
    public double LoadTimePercentage { get; set; }
    public int ServerResponseTime { get; set; }
    public double ServerResponsePercentage { get; set; }
    public double UptimePercentage { get; set; }
}

public record RecentPost
{
    public string Title { get; init; } = string.Empty;
    public DateTime PublishedAt { get; init; }
    public string Slug { get; init; } = string.Empty;
    public int ViewCount { get; init; }
}

public record ActivityItem
{
    public string Description { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; }
    public ActivityType Type { get; init; }
}

public enum ActivityType
{
    Post,
    Comment,
    Media,
    Tag
}