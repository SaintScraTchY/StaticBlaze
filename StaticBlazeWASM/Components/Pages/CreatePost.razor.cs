using System.Net.Http.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using StaticBlazeWASM.Models;

namespace StaticBlazeWASM.Components.Pages;

public partial class CreatePost : ComponentBase
{
    private Post Post { get; set; } = new Post();
    private string ThumbnailPreviewUrl { get; set; }
    private List<string> ImageUrls { get; set; } = new();

    private async Task HandleThumbnailUpload(InputFileChangeEventArgs e)
    {
        var file = e.File;
        var buffer = new byte[file.Size];
        await file.OpenReadStream().ReadAsync(buffer);
        ThumbnailPreviewUrl = $"data:{file.ContentType};base64,{Convert.ToBase64String(buffer)}";
        Post.ThumbnailBase64 = ThumbnailPreviewUrl;
    }

    private async Task HandleSubmit()
    {
        ImageUrls = ExtractImageUrls(Post.Content);
        var markdown = GenerateMarkdownWithMetadata();
        await Http.PostAsJsonAsync("api/posts", new { Content = markdown });
        Navigation.NavigateTo($"/post/{Post.Slug}");
    }

    private List<string> ExtractImageUrls(string markdown)
    {
        var pattern = @"!\[.*?\]\((.*?)\)";
        var matches = Regex.Matches(markdown, pattern);
        return matches.Select(m => m.Groups[1].Value).ToList();
    }

    private string GenerateMarkdownWithMetadata()
    {
        return $"""
        ---
        thumbnail: {Post.ThumbnailBase64}
        slug: {Post.Slug}
        title: {Post.Title}
        author: {Post.Author}
        tags: {Post.Tags}
        short-description: {Post.ShortDescription}
        ---
        {Post.Content}
        """;
    }

    private void ReplaceImage(string oldUrl)
    {
        // Implement image replacement logic
    }
    
}