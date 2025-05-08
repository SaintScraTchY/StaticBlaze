namespace StaticBlazeWASM.Constants;

public static class StaticBlazeConfig
{
    public static string BaseUrl { get; set; }
    
    public const string ImagePattern = @"!\[.*?\]\(data:image\/(png|jpeg|jpg|gif);base64,([A-Za-z0-9+/=]+)\)";
    public const string BlogAssets = @"StaticBlazeWASM/Blog/assets";
    public const string BlogCategories = @"StaticBlazeWASM/Blog/Categories";
    public const string BlogTags = @"StaticBlazeWASM/Blog/Tags";
    public const string BlogPosts = @"StaticBlazeWASM/Blog/Posts";
    public const string BlogDocs = @"StaticBlazeWASM/Blog/Docs";
    public const string BlogPages = @"StaticBlazeWASM/Blog/Pages";
    public const string BlogDB = @"StaticBlazeWASM/Blog/Persistence";
}