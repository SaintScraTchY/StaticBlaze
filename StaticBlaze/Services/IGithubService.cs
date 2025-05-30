using StaticBlaze.Models;

namespace StaticBlaze.Services;

public interface IGithubService
{
    Task<bool> ProcessMarkDown(BlogPost metaPost, string message,string fileName);
    Task<string> UploadImageToGitHub(byte[] imageBytes, string fileName);
    Task<int> GetTotalPosts();
    Task<DateTime?> GetLastCommitDate();
}