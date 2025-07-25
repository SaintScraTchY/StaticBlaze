namespace StaticBlaze.Constants;

public static class StaticBlazeConfig
{
    public static string BaseUrl { get; set; }
    
    public const string MarkDownImagePattern = @"!\[.*?\]\(data:image\/(png|jpeg|jpg|gif);base64,([A-Za-z0-9+/=]+)\)";
    public const string Base64ImagePattern = @"data:image\/(png|jpeg|jpg|gif);base64,([A-Za-z0-9+/=]+)";
    public const string ProjectName = "StaticBlaze/";
    public const string BlogAssets = @"Data/assets";
    public const string BlogCategories = @"Data/Categories.json";
    public const string BlogTags = @"Data/Tags.json";
    public const string BlogPosts = @"Data/Posts";
    public const string BlogDocs = @"Data/Docs";
    public const string BlogPages = @"Data/Pages";
    public const string BlogDB = @"Data/Persistence";
    
}