namespace StaticBlazeWASM.Constants;

public static class StaticBlazeConfig
{
    public static string BaseUrl { get; set; }
    
    public const string ImagePattern = @"!\[.*?\]\(data:image\/(png|jpeg|jpg|gif);base64,([A-Za-z0-9+/=]+)\)";
    public const string BlogAssets = @"StaticBlazeWASM/wwwroot/Static/Blog/assets";
    public const string BlogCategories = @"StaticBlazeWASM/wwwroot/Static/Blog/Categories";
    public const string BlogTags = @"StaticBlazeWASM/wwwroot/Static/Blog/Tags";
    public const string BlogPosts = @"StaticBlazeWASM/wwwroot/Static/Blog/Posts";
    public const string BlogDocs = @"StaticBlazeWASM/wwwroot/Static/Blog/Docs";
    public const string BlogPages = @"StaticBlazeWASM/wwwroot/Static/Blog/Pages";
    public const string BlogDB = @"StaticBlazeWASM/wwwroot/Static/Blog/Persistence";
}