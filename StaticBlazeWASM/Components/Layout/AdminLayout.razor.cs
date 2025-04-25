using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;

namespace StaticBlazeWASM.Components.Layout;

public partial class AdminLayout(ILocalStorageService LocalStorage,NavigationManager Navigation) : LayoutComponentBase
{
    private bool _isDarkMode = false;

    private bool isSidebarOpen = true;

    private void ToggleSidebar()
    {
        isSidebarOpen = !isSidebarOpen;
    }
    
    protected override async Task OnInitializedAsync()
    {
        if (!(await LocalStorage.ContainKeyAsync("theme")))
        {
            await LocalStorage.SetItemAsStringAsync("theme", "light");
        }
        else
        {
            var theme = await LocalStorage.GetItemAsStringAsync("theme");

            _isDarkMode = theme == "dark";
        }
    }
    private async Task ToggleDarkMode()
    {
        _isDarkMode = !_isDarkMode;
        var theme = await LocalStorage.GetItemAsStringAsync("theme");

        if (theme == "dark")
        {
            await LocalStorage.SetItemAsStringAsync("theme", "light");
        }
        else
        {
            await LocalStorage.SetItemAsStringAsync("theme", "dark");
        }
    }
    
    private void Logout()
    {
        Navigation.NavigateTo("/logout", true);
    }
}