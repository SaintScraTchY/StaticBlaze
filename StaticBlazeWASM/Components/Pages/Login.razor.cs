using Microsoft.AspNetCore.Components;
using StaticBlazeWASM.Models;

namespace StaticBlazeWASM.Components.Pages;

public partial class Login : ComponentBase
{
    private IEnumerable<Post> BlogPosts { get; set; } = new List<Post>();
}