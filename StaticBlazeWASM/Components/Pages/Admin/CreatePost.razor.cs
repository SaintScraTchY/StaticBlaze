using System.Security.Cryptography;
using System.Text.RegularExpressions;
using HeyRed.Mime;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Webp;
using StaticBlazeWASM.Constants;
using StaticBlazeWASM.Models;
using StaticBlazeWASM.Services;

namespace StaticBlazeWASM.Components.Pages.Admin;

public partial class CreatePost : ComponentBase
{
    private readonly IGithubService _githubService;
    
    private int uploadProgress = 0;
    private string uploadStatusMessage = "Starting upload...";

    public CreatePost(IGithubService githubService)
    {
        _githubService = githubService;
    }
    
    private BlogPost BlogPost { get; set; } = new BlogPost();
    private string? ThumbnailPreviewUrl { get; set; }
    
    private async Task HandleThumbnailUpload(InputFileChangeEventArgs e)
    {
        var file = e.File;

        if (file == null || file.Size == 0)
            return;

        await using var originalStream = file.OpenReadStream(maxAllowedSize: 5 * 1024 * 1024); // 5MB max

        // Compress image (adjust quality if needed)
        var compressedBytes = await CompressImageAsync(originalStream, file.ContentType, quality: 0.7);

        var base64 = Convert.ToBase64String(compressedBytes);
        ThumbnailPreviewUrl = $"data:{file.ContentType};base64,{base64}";

        // Store compressed result
        BlogPost.Thumbnail = ThumbnailPreviewUrl;
    }


    private async Task HandleSubmit()
    {
        var imageUrls = ExtractImageUrlsByString(BlogPost.Thumbnail);
        
        if(imageUrls == null || imageUrls.Count == 0)
        {
            return;
        }
        
        var imageBytes = Convert.FromBase64String(imageUrls.FirstOrDefault());
        var hash = ComputeImageHash(imageBytes);
        var fileName = $"{hash}.jpg"; // Use computed hash for uniqueness

        // Upload image to GitHub and get the URL
        var githubUrl = await _githubService.UploadImageToGitHub(imageBytes, fileName);
        
        BlogPost.Thumbnail = githubUrl;

        await _githubService.ProcessMarkDown(BlogPost, $"Create post {BlogPost.Slug}", BlogPost.Slug);
        Navigation.NavigateTo($"/post/{BlogPost.Slug}");
    }
    
    [JSInvokable]
    public async Task<string> HandleImageUpload(int[] imageBytes, string contentType)
    {
        // Convert the byte array to a byte[]
        var byteArray = imageBytes.Select(b => (byte)b).ToArray();
        var hash = ComputeImageHash(byteArray);

        // Get file extension from MIME type
        var extension = MimeTypesMap.GetExtension(contentType) ?? "bin";
        var fileName = $"{hash}.{extension}";

        // Upload to GitHub
        return await _githubService.UploadImageToGitHub(byteArray, fileName);
    }

    private static async Task<byte[]> CompressImageAsync(Stream imageStream, string contentType, double quality = 0.8)
    {
        using var image = await Image.LoadAsync(imageStream);

        await using var ms = new MemoryStream();

        if (contentType == "image/png")
        {
            var encoder = new PngEncoder { CompressionLevel = PngCompressionLevel.Level6 };
            await image.SaveAsync(ms, encoder);
        }
        else if (contentType == "image/webp")
        {
            var encoder = new WebpEncoder { Quality = (int)(quality * 100) };
            await image.SaveAsync(ms, encoder);
        }
        else
        {
            var encoder = new JpegEncoder { Quality = (int)(quality * 100) };
            await image.SaveAsync(ms, encoder);
        }

        return ms.ToArray();
    }

    private void ReplaceImage(string base64Image,string imageUrl) 
        => BlogPost.Content = BlogPost.Content.Replace(base64Image, imageUrl);

    private void ReplaceThisImage(string base64Image)
    {
        var imageBytes = Convert.FromBase64String(base64Image);
        //var compressedBytes = await CompressImageAsync(imageBytes, 0.7);
        var hash = ComputeImageHash(imageBytes);
        //Generate Image URL
        ReplaceImage(base64Image, GenerateGitHubImageUrl(hash + ".jpg"));
    }
    private static List<string> ExtractImageUrlsByMarkDown(string? markdown)
    {
        var matches = MarkDownImageExtractorRegex().Matches(markdown);
        return matches.Select(m => m.Groups[2].Value).ToList();
    }
    
    private static List<string> ExtractImageUrlsByString(string? markdown)
    {
        var matches = Bas64ImageExtractorRegex().Matches(markdown);
        return matches.Select(m => m.Groups[2].Value).ToList();
    }

    private static string ComputeImageHash(byte[] imageBytes)
        => Convert.ToHexStringLower(SHA256.HashData(imageBytes));
    
    private static string GenerateGitHubImageUrl(string fileName)
        => $"https://raw.githubusercontent.com/{GithubConfig.Owner}/{GithubConfig.Repo}/{GithubConfig.Branch}/images/{fileName}";

    [GeneratedRegex(StaticBlazeConfig.MarkDownImagePattern)]
    private static partial Regex MarkDownImageExtractorRegex();
    
    [GeneratedRegex(StaticBlazeConfig.Base64ImagePattern)]
    private static partial Regex Bas64ImageExtractorRegex();
}