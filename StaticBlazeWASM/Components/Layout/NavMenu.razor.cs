﻿using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace StaticBlazeWASM.Components.Layout;

public partial class NavMenu : ComponentBase
{
    [Inject]
    private ILocalStorageService LocalStorage { get; set; } = null!;
    
    [Inject]
    private IJSRuntime JSRuntime { get; set; } = null!;

    private bool _isMobileMenuOpen;

    [Parameter]
    public bool IsDarkMode { get; set; }

    [Parameter]
    public EventCallback<bool> IsDarkModeChanged { get; set; }

    private void ToggleMobileMenu()
    {
        _isMobileMenuOpen = !_isMobileMenuOpen;
    }

    protected override async Task OnInitializedAsync()
    {
        if (!(await LocalStorage.ContainKeyAsync("theme")))
        {
            IsDarkMode = false;
            await LocalStorage.SetItemAsStringAsync("theme", "light");
            await IsDarkModeChanged.InvokeAsync(IsDarkMode);
        }
        else
        {
            var theme = await LocalStorage.GetItemAsStringAsync("theme");
            IsDarkMode = theme == "dark";
            await IsDarkModeChanged.InvokeAsync(IsDarkMode);
        }
        
        await ApplyTheme(IsDarkMode);
    }
    
    private async Task ToggleDarkMode()
    {
        IsDarkMode = !IsDarkMode;
        await LocalStorage.SetItemAsStringAsync("theme", IsDarkMode ? "dark" : "light");
        await IsDarkModeChanged.InvokeAsync(IsDarkMode);
        await ApplyTheme(IsDarkMode);
    }
    
    private async Task ApplyTheme(bool isDark)
    {
        // Force theme by adding/removing class from html element
        await JSRuntime.InvokeVoidAsync("eval", $"document.documentElement.classList.{(isDark ? "add" : "remove")}('dark')");
    }
}

