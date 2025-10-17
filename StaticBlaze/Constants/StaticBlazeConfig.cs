namespace StaticBlaze.Constants;

public static class StaticBlazeConfig
{
    public static string BaseUrl { get; set; }
    
    public const string MarkDownImagePattern = @"!\[.*?\]\(data:image\/(png|jpeg|jpg|gif);base64,([A-Za-z0-9+/=]+)\)";
    public const string Base64ImagePattern = @"data:image\/(png|jpeg|jpg|gif);base64,([A-Za-z0-9+/=]+)";
    public const string ProjectName = "StaticBlaze/";
    public const string BlogAssets = @"assets";
    public const string BlogCategories = @"Categories.json";
    public const string BlogTags = @"Tags.json";
    public const string BlogAuthors = @"Authors.json";
    public const string BlogPosts = @"Posts";
    public const string BlogDocs = @"Docs";
    public const string BlogPages = @"Pages";
    public const string BlogDB = @"Persistence";
    
}