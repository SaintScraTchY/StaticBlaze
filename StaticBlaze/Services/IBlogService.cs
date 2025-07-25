﻿using StaticBlaze.Models;

namespace StaticBlaze.Services;

public interface IBlogService
{
    ValueTask<IList<BlogPost>> GetPosts(int page, int pageSize, string? search = null, string? tag = null, string? category = null);
    ValueTask<IList<BlogPost>> GetLandingPosts();
    Task<DashboardStats> GetDashboardStats();
    Task<List<MetaPost>> GetPostsAsync();
    Task<BlogPost?> GetPostAsync(string slug);
    Task<List<MetaPost>> GetRecentPosts(int count);
    Task<List<ActivityItem>> GetRecentActivity(int count);
    Task<bool> DeletePost(string id);
    Task<List<Author>?> GetAllAuthorsAsync();
    Task<Author?> GetAuthorByIdAsync(Guid id);
    Task<bool> CreateAuthorAsync(Author author);
    Task<bool> UpdateAuthorAsync(Author author);
    Task<bool> DeleteAuthorAsync(Guid id);
    Task<List<Tag>> GetTagsAsync();
    Task<bool> DeleteTagAsync(Guid id);
    Task<Tag> GetTagByIdAsync(Guid id);
    Task<bool> UpdateTagAsync(Tag tag);
    Task<bool> CreateTagAsync(Tag tag);
    Task<List<Category>?> GetCategoriesAsync();
    Task<bool> DeleteCategoryAsync(Guid id);
    Task<bool> UpdateCategoryAsync(Category category);
    Task<bool> CreateCategoryAsync(Category category);
    Task<Category> GetCategoryByIdAsync(Guid id);
    Task<string?> GetFileContentAsync(string filePath);
}