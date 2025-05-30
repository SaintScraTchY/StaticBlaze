using StaticBlaze.Models;

namespace StaticBlaze.Services;

public interface IBlogService
{
    Task<DashboardStats> GetDashboardStats();
    Task<List<MetaPost>> GetPostsAsync();
    Task<BlogPost?> GetPostAsync(string slug);
    Task<List<MetaPost>> GetRecentPosts(int count);
    Task<List<ActivityItem>> GetRecentActivity(int count);
    Task<bool> DeletePost(string id);
}