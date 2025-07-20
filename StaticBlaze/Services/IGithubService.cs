using Microsoft.AspNetCore.Components.Forms;
using StaticBlaze.Models;

namespace StaticBlaze.Services;

public interface IGithubService
{
    Task<bool> ProcessMarkDown(BlogPost metaPost, string message,string fileName);
    Task<string> UploadImageToGitHub(byte[] imageBytes, string fileName);
    Task<int> GetTotalPosts();
    Task<DateTime?> GetLastCommitDate();
    Task<string> UploadFileAsync(IBrowserFile file);
    Task<string> UploadFileAsync(Stream stream, string fileName);
    Task<string> UploadFileAsync(string base64Content, string fileName);
    Task<List<string>> UploadFilesAsync(IEnumerable<IBrowserFile> files);
}