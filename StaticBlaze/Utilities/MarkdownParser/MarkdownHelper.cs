using System.Text.RegularExpressions;
using Markdig;
using Markdig.Extensions.AutoIdentifiers;
using StaticBlaze.Models;

namespace StaticBlaze.Utilities.MarkdownParser;

public static partial class MarkdownHelper
{
    
    public static BlogPost ParseMarkdown(string markdown)
    {
        var match = Regex.Match(markdown, @"^---\s*\n(?<meta>.*?)\n---\s*\n(?<body>.*)", RegexOptions.Singleline);
        if (!match.Success) throw new Exception("Invalid frontmatter");

        var meta = match.Groups["meta"].Value;

        var dict = new Dictionary<string, string>();
        foreach (var line in meta.Split('\n', StringSplitOptions.RemoveEmptyEntries))
        {
            var kv = line.Split(':', 2);
            if (kv.Length == 2)
                dict[kv[0].Trim().ToLower()] = kv[1].Trim();
        }

        var blogPost = new BlogPost()
        {
            Title = dict.GetValueOrDefault("title"),
            Slug = dict.GetValueOrDefault("slug"),
            Author = dict.GetValueOrDefault("author"),
            Tags = dict.GetValueOrDefault("tags"),
            Thumbnail = dict.GetValueOrDefault("thumbnail"),
            ShortDescription = dict.GetValueOrDefault("short-description"),
        };

        return blogPost;
    }
    
    public static string ToHtml(string markdown)
    {
        
        var pipeline = new MarkdownPipelineBuilder()
            .UseYamlFrontMatter() // Allow frontmatter metadata parsing
            .UsePipeTables()
            .UseGridTables()
            .UseEmojiAndSmiley()
            .UseAutoIdentifiers(AutoIdentifierOptions.GitHub)
            .UseBootstrap() // Adds class attributes for tables, blockquotes, etc.
            .UseGenericAttributes() // Enable `{.class #id}` support
            .UseAdvancedExtensions()
            .Build();
        
        // Replace code block with mermaid div
        var rendered = Markdown.ToHtml(markdown, pipeline);
        rendered = MyRegex().Replace(rendered, "<div class=\"mermaid\">$1</div>");

        return Markdown.ToHtml(markdown, pipeline);
    }
    
    public static string GenerateMarkdownWithMetadata(this BlogPost post)
    {
        return $"""
                ---
                thumbnail: {post.Thumbnail}
                slug: {post.Slug}
                title: {post.Title}
                author: {post.Author}
                tags: {post.Tags}
                short-description: {post.ShortDescription}
                ---
                {post.Content}
                """;
    }

    [GeneratedRegex("<pre><code class=\"language-mermaid\">(.*?)</code></pre>", RegexOptions.Singleline)]
    private static partial Regex MyRegex();
}