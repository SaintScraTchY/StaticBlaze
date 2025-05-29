using StaticBlazeWASM.Models;

namespace StaticBlazeWASM.Services;

public interface IGithubService
{
    Task<bool> ProcessMarkDown(BlogPost metaPost, string message,string fileName);
    Task<string> UploadImageToGitHub(byte[] imageBytes, string fileName);
    Task<int> GetTotalPosts();
    Task<DateTime?> GetLastCommitDate();
}