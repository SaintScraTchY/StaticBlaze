using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
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
        var docResponse = _httpClient.GetStringAsync($"{_navigationManager.BaseUri}{StaticBlazeConfig.BlogDocs}/{slug}.md.gz");
        var postResponse = _httpClient.GetStringAsync($"{_navigationManager.BaseUri}{StaticBlazeConfig.BlogPosts}/{slug}.html.gz");
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
    
}