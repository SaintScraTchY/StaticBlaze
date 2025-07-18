using Humanizer;
using Microsoft.AspNetCore.Components;
using StaticBlaze.Models;
using StaticBlaze.Services;

namespace StaticBlaze.Components.Pages.Admin;

public partial class Dashboard : ComponentBase
{
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private IBlogService BlogService { get; set; } = default!;

    private List<MetaPost> RecentPosts { get; set; } = new();
    private List<ActivityItem> RecentActivities { get; set; } = new();
    private List<Author> Authors { get; set; } = new();
    private DashboardStats Stats { get; set; } = new();

    private string GetTimeAgo(DateTime date) => date.Humanize();

    protected override async Task OnInitializedAsync()
    {
        await LoadDashboardData();
    }

    private async Task LoadDashboardData()
    {
        var tasks = new List<Task>
        {
            LoadStats(),
            LoadRecentPosts(),
            LoadRecentActivities(),
            LoadAuthors()
        };

        await Task.WhenAll(tasks);
    }

    private async Task LoadStats()
    {
        Stats = await BlogService.GetDashboardStats();
    }

    private async Task LoadRecentPosts()
    {
        RecentPosts = await BlogService.GetRecentPosts(5);
    }

    private async Task LoadRecentActivities()
    {
        RecentActivities = await BlogService.GetRecentActivity(5);
    }

    private async Task LoadAuthors()
    {
        Authors = await BlogService.GetAllAuthorsAsync();
    }

    private void NavigateToEditPost(string slug) => Navigation.NavigateTo($"/admin/posts/edit/{slug}");
    private void NavigateToNewPost() => Navigation.NavigateTo("/admin/posts/new");
    private void NavigateToTags() => Navigation.NavigateTo("/admin/tags");
    private void NavigateToImages() => Navigation.NavigateTo("/admin/media");
    private void NavigateToSettings() => Navigation.NavigateTo("/admin/settings");
}
