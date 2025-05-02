using Markdig;
using Markdig.Extensions.AutoIdentifiers;

namespace StaticBlazeWASM.Utilities;

public static class MarkdownHelper
{
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

        return Markdown.ToHtml(markdown, pipeline);
    }

}