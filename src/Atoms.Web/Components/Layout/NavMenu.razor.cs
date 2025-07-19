using Microsoft.AspNetCore.Components.Routing;

namespace Atoms.Web.Components.Layout;

public partial class NavMenuComponent : Component2Base, IDisposable
{
    protected string? CurrentUrl;
    protected string? UserName;

    [Inject]
    public NavigationManager NavigationManager { get; set; } = default!;

    protected async override Task OnInitializedAsync()
    {
        CurrentUrl = NavigationManager.ToBaseRelativePath(NavigationManager.Uri);
        UserName = await GetUserName();

        NavigationManager.LocationChanged += OnLocationChanged;
    }
    private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        CurrentUrl = NavigationManager.ToBaseRelativePath(e.Location);

        StateHasChanged();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            NavigationManager.LocationChanged -= OnLocationChanged;
        }
    }
}