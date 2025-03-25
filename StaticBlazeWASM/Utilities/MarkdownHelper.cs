using Markdig;

namespace StaticBlazeWASM.Utilities;

public static class MarkdownHelper
{
    public static string ToHtml(string markdown)
    {
        var pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions() // Enable extensions like tables, task lists, etc.
            .Build();

        return Markdown.ToHtml(markdown, pipeline);
    }
}