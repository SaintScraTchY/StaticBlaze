using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using StaticBlazeWASM.Constants;
using StaticBlazeWASM.Models;
using StaticBlazeWASM.Utilities;

namespace StaticBlazeWASM.Services;

public class BlogService
{
    private readonly HttpClient _httpClient;
    private readonly NavigationManager _navigationManager;

    public BlogService(HttpClient httpClient, NavigationManager navigationManager)
    {
        _httpClient = httpClient;
        _navigationManager = navigationManager;
    }

    public async Task<List<MetaPost>> GetPostsAsync()
    {
        var response = await _httpClient.GetAsync($"/posts");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<MetaPost>>();
    }

    public async Task<BlogPost?> GetPostAsync(string slug)
    {
        var docResponse = _httpClient.GetAsync($"{_navigationManager.BaseUri}/{StaticBlazeConfig.BlogDocs}/{slug}.md");
        var postResponse = _httpClient.GetAsync($"{_navigationManager.BaseUri}/{StaticBlazeConfig.BlogPosts}/{slug}.html");
        await Task.WhenAll(docResponse, postResponse);
        docResponse.Result.EnsureSuccessStatusCode();
        postResponse.Result.EnsureSuccessStatusCode();

        var postSummary = await docResponse.Result.Content.ReadAsStringAsync();
        var postContent = await postResponse.Result.Content.ReadAsStringAsync();
        if (string.IsNullOrEmpty(postSummary) || string.IsNullOrEmpty(postContent))
        {
            return null;
        }

        var post = MarkdownHelper.ParseMarkdown(postSummary);
        post.Content = postContent;
        return post;
    }
    
}