namespace StaticBlaze.Constants;

public static class StaticBlazeConfig
{
    public static string BaseUrl { get; set; }
    
    public const string MarkDownImagePattern = @"!\[.*?\]\(data:image\/(png|jpeg|jpg|gif);base64,([A-Za-z0-9+/=]+)\)";
    public const string Base64ImagePattern = @"data:image\/(png|jpeg|jpg|gif);base64,([A-Za-z0-9+/=]+)";
    public const string ProjectName = "StaticBlazeWASM/";
    public const string BlogAssets = @"Blog/assets";
    public const string BlogCategories = @"Blog/Categories";
    public const string BlogTags = @"Blog/Tags";
    public const string BlogPosts = @"Blog/Posts";
    public const string BlogDocs = @"Blog/Docs";
    public const string BlogPages = @"Blog/Pages";
    public const string BlogDB = @"Blog/Persistence";
}