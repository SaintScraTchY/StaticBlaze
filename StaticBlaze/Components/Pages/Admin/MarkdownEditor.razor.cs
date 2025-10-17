using System.Security.Cryptography;
using HeyRed.Mime;
using Microsoft.JSInterop;
using StaticBlaze.Constants;
using StaticBlaze.Services;

namespace StaticBlaze.Components.Pages.Admin;

public partial class MarkdownEditor : ComponentBase
{
    private ElementReference EditorElement;
    // private IJSObjectReference _mdModule;
    private IJSObjectReference _module;
    private IJSObjectReference _editor;
    private bool _isInitialized;
    private readonly IGithubService _githubService;


    [Parameter] public string Content { get; set; } = string.Empty;
    [Parameter] public EventCallback<string> ContentChanged { get; set; }

    public MarkdownEditor(IGithubService githubService)
    {
        _githubService = githubService;
    }
    protected override async Task OnAfterRenderAsync(bool first)
    {
        if (first)
        {
            _module = await JsRuntime.InvokeAsync<IJSObjectReference>("import", "./js/MarkdownEditor.js");
            _editor = await _module.InvokeAsync<IJSObjectReference>("initEditor", EditorElement, DotNetObjectReference.Create(this), Content);
            _isInitialized = true;
            StateHasChanged();
        }
    }
    
    private static string ComputeImageHash(byte[] imageBytes)
        => Convert.ToHexStringLower(SHA256.HashData(imageBytes));
    
    private static string GenerateGitHubImageUrl(string fileName)
        => $"https://raw.githubusercontent.com/{GithubConfig.Owner}/{GithubConfig.Repo}/{GithubConfig.Branch}/images/{fileName}";
    
    [JSInvokable]
    public async Task<string> HandleImageUpload(int[] imageBytes, string contentType)
    {
        // Convert the byte array to a byte[]
        var byteArray = imageBytes.Select(b => (byte)b).ToArray();
        var hash = ComputeImageHash(byteArray);

        // Get file extension from MIME type
        var extension = MimeTypesMap.GetExtension(contentType) ?? "bin";
        var fileName = $"{hash}.{extension}";

        // Upload to GitHub
        return await _githubService.UploadImageToGitHub(byteArray, fileName);
    }

    [JSInvokable]
    public async Task UpdateEditorValue(string markdown)
    {
        Content = markdown;
        await ContentChanged.InvokeAsync(markdown);
    }

    public async ValueTask DisposeAsync()
    {
        if (_editor != null)
        {
             await _editor.InvokeVoidAsync("destroy");
             await _editor.DisposeAsync();
        }

        if (_module != null)
        {
            await _module.DisposeAsync();
        }
    }
}