using Microsoft.AspNetCore.Components;
using Humanizer;
using StaticBlazeWASM.Models;
using StaticBlazeWASM.Services;

namespace StaticBlazeWASM.Components.Pages.Admin;

public partial class Dashboard : ComponentBase
{
    [Inject] private NavigationManager Navigation { get; set; }
    [Inject] private IBlogService _blogService { get; set; }

    private List<MetaPost> RecentPosts { get; set; } = new();
    private List<ActivityItem> RecentActivities { get; set; } = new();
    private DashboardStats Stats { get; set; } = new();

    private string GetTimeAgo(DateTime date) => date.Humanize();

    private List<ActivityLog> RecentActivity { get; set; } = new();

    [Inject] private IBlogService BlogService { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        await LoadDashboardData();
    }

    private async Task LoadDashboardData()
    {
        Stats = await BlogService.GetDashboardStats();
        RecentPosts = await BlogService.GetRecentPosts(5);
        RecentActivities = await BlogService.GetRecentActivity(5);
    }

    private void NavigateToEditPost(string postId) => Navigation.NavigateTo($"/Admin/Posts/Edit/{postId}");
    private void NavigateToNewPost() => Navigation.NavigateTo("/admin/posts/new");
    private void NavigateToTags() => Navigation.NavigateTo("/admin/tags");
    private void NavigateToImages() => Navigation.NavigateTo("/admin/media");
    private void NavigateToSettings() => Navigation.NavigateTo("/admin/settings");

}
