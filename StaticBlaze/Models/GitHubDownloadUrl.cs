using System.Text.Json.Serialization;

namespace StaticBlaze.Models;

public record GitHubDownloadUrl
{
    [JsonPropertyName("download_url")]
    public string DownloadUrl { get; set; }
};
