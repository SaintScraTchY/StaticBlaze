using System.Text.RegularExpressions;
using Markdig;
using Markdig.Extensions.AutoIdentifiers;
using StaticBlaze.Models;

namespace StaticBlaze.Utilities.MarkdownParser;

public static partial class MarkdownHelper
{
    
    public static BlogPost ParseMarkdown(this string markdown)
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
    
    public static string ToHtml(this string markdown)
    {
        var pipeline = new MarkdownPipelineBuilder()
            .UseYamlFrontMatter()
            .UsePipeTables()
            .UseGridTables()
            .UseEmojiAndSmiley()
            .UseAutoIdentifiers(AutoIdentifierOptions.GitHub)
            .UseGenericAttributes()
            .UseAdvancedExtensions()
            .UseTaskLists()
            .UseCitations()
            .UseFootnotes()
            .UseFooters()
            .Build();

        var text = PreprocessMermaid(markdown);
        return Markdown.ToHtml(text, pipeline);
    }

    
    private static string PreprocessMermaid(string markdown)
    {
        return Regex.Replace(
            markdown,
            @"```(?:mermaid|graph|sequenceDiagram|gantt|classDiagram)\s+(.*?)```",
            match =>
            {
                var content = match.Groups[1].Value;
                return $"<pre class=\"mermaid\">\n{content.Trim()}\n</pre>";
            },
            RegexOptions.Singleline | RegexOptions.Compiled);
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