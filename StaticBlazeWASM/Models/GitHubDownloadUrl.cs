using System.Text.Json.Serialization;

namespace StaticBlazeWASM.Models;

public record GitHubDownloadUrl
{
    [JsonPropertyName("download_url")]
    public string DownloadUrl { get; set; }
};
