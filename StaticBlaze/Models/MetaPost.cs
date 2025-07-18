using System.ComponentModel.DataAnnotations;

namespace StaticBlaze.Models;

public class MetaPost
{
    public Guid Guid { get; init; }
    [Required(ErrorMessage = "Slug is required")]
    public string Slug { get; set; }

    public bool Featured { get; set; }

    [Required]
    public string? Title { get; set; }

    [Required, MaxLength(256)]
    public string ShortDescription { get; set; }

    public DateTime CreatedDateTime { get; set; }
    public DateTime ModifiedDateTime { get; set; }
    public DateTime PublishedAt { get; set; }

    public string Author { get; set; }
    public string Tags { get; set; }
    public string Category { get; set; }
    public int ReadTime { get; set; }
    public string? Thumbnail { get; set; }
}
