using System.ComponentModel.DataAnnotations;

namespace StaticBlaze.Models;

public class Tag
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required(ErrorMessage = "Title is required")]
    public string Title { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Slug is required")]
    [RegularExpression(@"^[a-z0-9]+(?:-[a-z0-9]+)*$", ErrorMessage = "Slug must be lowercase and contain only letters, numbers, and hyphens")]
    public string Slug { get; set; } = string.Empty;
    
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    public List<MetaPost> Posts { get; set; } = [];
}