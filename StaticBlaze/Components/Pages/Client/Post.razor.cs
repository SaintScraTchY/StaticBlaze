using Microsoft.JSInterop;
using StaticBlaze.Models;
using StaticBlaze.Services;

namespace StaticBlaze.Components.Pages.Client;

public partial class Post : ComponentBase
{
    public Post(IBlogService blogService)
    {
        _blogService = blogService;
    }

    [Parameter] public string Slug { get; set; }
    private BlogPost? _blogPost;
    private readonly IBlogService _blogService;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        _blogPost = await _blogService.GetPostAsync(Slug);

        Console.WriteLine("the Thumbnail is: " + _blogPost?.Thumbnail);

        if (firstRender && !string.IsNullOrEmpty(_blogPost?.Content))
        {
            await JsRuntime.InvokeVoidAsync("renderMarkdown", _blogPost.Content, "postContent");
        }
        await base.OnAfterRenderAsync(firstRender);
    }
}