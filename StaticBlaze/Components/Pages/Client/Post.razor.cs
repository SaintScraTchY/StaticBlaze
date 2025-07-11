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
    
    private ElementReference MarkdownContainer;
    private IJSObjectReference? _mermaidInterop;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _mermaidInterop = await JsRuntime.InvokeAsync<IJSObjectReference>("import", "/js/mermaid-init.js");
            await _mermaidInterop.InvokeVoidAsync("initMermaid");
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_mermaidInterop is not null)
        {
            await _mermaidInterop.DisposeAsync();
        }
    }
    
    protected override async Task OnParametersSetAsync()
    {
        _blogPost = await _blogService.GetPostAsync(Slug) ?? new BlogPost();
        Console.WriteLine("the Thumbnail is : "+ _blogPost.Thumbnail);
        await base.OnParametersSetAsync();
    }
}