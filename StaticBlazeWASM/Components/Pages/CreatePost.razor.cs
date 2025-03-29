using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using StaticBlazeWASM.Constants;
using StaticBlazeWASM.Models;
using StaticBlazeWASM.Services;

namespace StaticBlazeWASM.Components.Pages;

public partial class CreatePost : ComponentBase
{
    private readonly GithubService _githubService;
    
    private int uploadProgress = 0;
    private string uploadStatusMessage = "Starting upload...";

    public CreatePost(GithubService githubService)
    {
        Console.WriteLine($"HostEnvBase : {GithubConfig.Branch}");
        _githubService = githubService;
    }

    private Post Post { get; set; } = new Post();
    private string ThumbnailPreviewUrl { get; set; }
    private List<string> ImageUrls { get; set; } = new();
    
    private async Task HandleThumbnailUpload(InputFileChangeEventArgs e)
    {
        var file = e.File;
        await using var stream = file.OpenReadStream();
        var imageBytes = await CompressImageAsync(stream, 0.7); // Compress Before Encoding
        ThumbnailPreviewUrl = $"data:{file.ContentType};base64,{Convert.ToBase64String(imageBytes)}";
        Post.ThumbnailBase64 = ThumbnailPreviewUrl;
    }

    private async Task HandleSubmit()
    {
        ImageUrls = ExtractImageUrls(Post.Content);

        foreach (var base64Image in ImageUrls)
        {
            var imageBytes = Convert.FromBase64String(base64Image);
            var hash = ComputeImageHash(imageBytes);
            var fileName = $"{hash}.jpg"; // Use computed hash for uniqueness

            // Upload image to GitHub and get the URL
            var githubUrl = await _githubService.UploadImageToGitHub(imageBytes, fileName);

            if (!string.IsNullOrEmpty(githubUrl))
            {
                ReplaceImage(base64Image, githubUrl);
            }
        }

        var markdown = GenerateMarkdownWithMetadata();
        await _githubService.ProcessMarkDown(markdown, $"Create post {Post.Slug}", Post.Slug);
        Navigation.NavigateTo($"/post/{Post.Slug}");
    }

    private static async Task<byte[]> CompressImageAsync(Stream imageStream, double quality)
    {
        using var image = await Image.LoadAsync(imageStream);
        var encoder = new JpegEncoder { Quality = (int)(quality * 100) };
        using var ms = new MemoryStream();
        await image.SaveAsync(ms, encoder);
        return ms.ToArray();
    }

    private static async Task<byte[]> CompressImageAsync(byte[] originalBytes, double quality)
    {
        using var ms = new MemoryStream(originalBytes);
        return await CompressImageAsync(ms, quality);
    }

    private void ReplaceImage(string base64Image,string imageUrl) 
        => Post.Content = Post.Content.Replace(base64Image, imageUrl);

    private void ReplaceThisImage(string base64Image)
    {
        var imageBytes = Convert.FromBase64String(base64Image);
        //var compressedBytes = await CompressImageAsync(imageBytes, 0.7);
        var hash = ComputeImageHash(imageBytes);
        //Generate Image URL
        ReplaceImage(base64Image, GenerateGitHubImageUrl(hash + ".jpg"));
    }
    private static List<string> ExtractImageUrls(string markdown)
    {
        var matches = ImageExtracterRegex().Matches(markdown);
        return matches.Select(m => m.Groups[2].Value).ToList();
    }

    private static string ComputeImageHash(byte[] imageBytes)
        => Convert.ToHexStringLower(SHA256.HashData(imageBytes));

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
    
    private static string GenerateGitHubImageUrl(string fileName)
        => $"https://raw.githubusercontent.com/{GithubConfig.Owner}/{GithubConfig.Repo}/{GithubConfig.Branch}/images/{fileName}";

    [GeneratedRegex(StaticBlazeConfig.ImagePattern)]
    private static partial Regex ImageExtracterRegex();
}