using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;

namespace StaticBlaze.Components.Layout;

public partial class AdminLayout : LayoutComponentBase
{
    [Inject] private ILocalStorageService LocalStorage { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    
    private bool _isDarkMode;
    private bool _isSidebarOpen = true;
    private bool _isTransitioning;
    
    private string GetNavLinkClass => 
        $"flex items-center space-x-3 p-2 rounded-lg transition-colors {(_isSidebarOpen ? "px-3" : "justify-center")} " +
        "text-gray-600 dark:text-gray-400 hover:bg-gray-100 dark:hover:bg-gray-700 hover:text-gray-900 dark:hover:text-white";

    protected override async Task OnInitializedAsync()
    {
        await LoadThemePreference();
        await LoadSidebarState();
    }

    private async Task LoadThemePreference()
    {
        if (!(await LocalStorage.ContainKeyAsync("theme")))
        {
            await LocalStorage.SetItemAsStringAsync("theme", "light");
            // Check system preference for dark mode
            var darkModeQuery = await LocalStorage.GetItemAsync<bool>("systemPrefersDark");
            if (darkModeQuery)
            {
                _isDarkMode = true;
                await LocalStorage.SetItemAsStringAsync("theme", "dark");
            }
        }
        else
        {
            var theme = await LocalStorage.GetItemAsStringAsync("theme");
            _isDarkMode = theme == "dark";
        }
    }
    
    
    private async Task ToggleSidebar()
    {
        _isSidebarOpen = !_isSidebarOpen;
        await LocalStorage.SetItemAsync("sidebarOpen", _isSidebarOpen);
        StateHasChanged();
    }

    private async Task LoadSidebarState()
    {
        if (await LocalStorage.ContainKeyAsync("sidebarOpen"))
        {
            _isSidebarOpen = await LocalStorage.GetItemAsync<bool>("sidebarOpen");
        }
    }

    private async Task ToggleDarkMode()
    {
        _isDarkMode = !_isDarkMode;
        await LocalStorage.SetItemAsStringAsync("theme", _isDarkMode ? "dark" : "light");
        StateHasChanged();
    }

    private async Task Logout()
    {
        await LocalStorage.RemoveItemAsync("authToken");
        await LocalStorage.RemoveItemAsync("user");
        Navigation.NavigateTo("/logout", true);
    }
}

