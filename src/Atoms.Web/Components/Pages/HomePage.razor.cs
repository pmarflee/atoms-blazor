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
            var storageId = await BrowserStorageService.GetOrAddStorageId();
            var request = new CreateNewGameRequest(
                GameMenuOptions.CreateForDebug(
                Guid.NewGuid(),
                storageId,
                AuthenticatedUser.GetUserId()));
            var response = await Mediator.Send(request);

            StartGame(response.Game);
        }
        else
        {
            await StateContainer.SetMenu();
        }
    }

    protected void OnCreateGame(Game game)
    {
        StartGame(game);
    }

    void StartGame(Game game)
    {
        Navigation.NavigateToGame(game, Debug);
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