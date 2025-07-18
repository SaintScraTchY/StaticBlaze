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

    private ElementReference _markdownContainer;
    private bool _contentRendered;
    private IJSObjectReference? _module;
    private DotNetObjectReference<Post>? _objectReference;

    // protected override async Task OnAfterRenderAsync(bool firstRender)
    // {
    //     if (firstRender)
    //     {
    //         // Load markdown dependencies first
    //         await JsRuntime.InvokeVoidAsync("loadMarkdownDependencies");
    //         _module = await JsRuntime.InvokeAsync<IJSObjectReference>("import", "./Js/post-markdown.js");
    //         _objectReference = DotNetObjectReference.Create(this);
    //     }
    //
    //     if (_blogPost != null && !_contentRendered && _module != null)
    //     {
    //         await RenderMarkdownContent();
    //     }
    // }
    
    protected override async Task OnParametersSetAsync()
    {
        _blogPost = await _blogService.GetPostAsync(Slug);
    }
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_blogPost != null && !_contentRendered)
        {
            try 
            {
                await JsRuntime.InvokeVoidAsync("renderMarkdown", _blogPost.Content, "postContent");
                _contentRendered = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error rendering markdown: {ex.Message}");
            }
        }
    }

    private async Task RenderMarkdownContent()
    {
        if (_module != null && _blogPost != null)
        {
            try 
            {
                await _module.InvokeVoidAsync("renderMarkdown", _markdownContainer, _blogPost.Content);
                _contentRendered = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error rendering markdown: {ex.Message}");
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_module is not null)
        {
            await _module.DisposeAsync();
        }
        _objectReference?.Dispose();
    }
}