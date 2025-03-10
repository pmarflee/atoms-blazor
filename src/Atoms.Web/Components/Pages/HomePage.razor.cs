using Atoms.UseCases.CreateNewGame;

using Game = Atoms.Core.Entities.Game;

namespace Atoms.Web.Components.Pages;

public partial class HomePageComponent : Component2Base, IDisposable
{
    [Inject]
    NavigationManager Navigation { get; set; } = default!;

    [Inject]
    GameStateContainer StateContainer { get; set; } = default!;

    [Inject]
    BrowserStorageService BrowserStorageService { get; set; } = default!;

    [CascadingParameter]
    ClaimsPrincipal? AuthenticatedUser { get; set; }

    [SupplyParameterFromQuery]
    protected int? Debug { get; set; }

    protected async override Task OnInitializedAsync()
    {
        StateContainer.OnChange += StateHasChangedAsync;

        if (Debug.HasValue)
        {
            Navigation.NavigateToDebugGame(Debug.Value);
        }
        else
        {
            await StateContainer.SetMenu();
        }
    }

    protected void OnCreateGame(Game game)
    {
        Navigation.NavigateToGame(game);
    }

    async Task StateHasChangedAsync()
    {
        await InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            StateContainer.OnChange -= StateHasChangedAsync;
        }
    }
}