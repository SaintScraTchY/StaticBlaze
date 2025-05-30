
namespace StaticBlaze.Models;

public class ActivityLog
{
    public string Message { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Type { get; set; } = string.Empty;
}
