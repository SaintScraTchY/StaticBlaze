using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using StaticBlazeWASM.Models;

namespace StaticBlazeWASM.Components.Pages.Client;

public partial class Post : ComponentBase
{
    [Parameter] public string Slug { get; set; }
    private BlogPost? blogPost;

    protected override async Task OnParametersSetAsync()
    {
        blogPost = await BlogService.GetPostAsync(Slug) ?? new BlogPost();
        await base.OnParametersSetAsync();
    }
}