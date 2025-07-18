using System.ComponentModel.DataAnnotations;

namespace StaticBlaze.Models;

public class Author
{
    public Guid Id { get; set; }
    
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Username is required")]
    public string Username { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; } = string.Empty;
    
    public string? ProfilePictureUrl { get; set; }
    
    [Required(ErrorMessage = "About section is required")]
    public string AboutMe { get; set; } = string.Empty;
    
    public string? Website { get; set; }
    
    public List<string> SocialLinks { get; set; } = new();
    
    public List<string> Expertise { get; set; } = new();
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? LastUpdated { get; set; }
    
    public List<string> PostIds { get; set; } = new();
}
