using System;
using System.ComponentModel.DataAnnotations;

namespace StaticBlazeWASM.Models;

public class MetaPost
{
    public Guid Guid { get; init; }
    [Required(ErrorMessage = "Slug is required")]
    [RegularExpression(@"^[a-z0-9-]+$", ErrorMessage = "Only lowercase letters, numbers, and hyphens")]
    public string Slug { get; set; }

    [Required]
    public string? Title { get; set; }

    [Required, MaxLength(256)]
    public string ShortDescription { get; set; }

    public DateTime CreatedDateTime { get; set; }
    public DateTime EditedDateTime { get; set; }
    public DateTime PublishedAt { get; set; }
    

    public string Author { get; set; }
    public string Tags { get; set; }
    public int ReadTime { get; set; }
    public string? Thumbnail { get; set; }
}