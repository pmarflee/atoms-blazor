using Atoms.UseCases.GetGame;

namespace Atoms.Web.Components.Pages;

public partial class GameComponent : Component2Base, IDisposable
{
    [Inject]
    NavigationManager Navigation { get; set; } = default!;

    [Inject]
    BrowserStorageService BrowserStorageService { get; set; } = default!;

    [Inject]
    GameStateContainer StateContainer { get; set; } = default!;

    [Inject]
    IJSRuntime JSRuntime { get; set; } = default!;

    [CascadingParameter]
    ClaimsPrincipal? AuthenticatedUser { get; set; }

    [Parameter]
    public Guid GameId { get; set; }

    [SupplyParameterFromQuery]
    protected int? Debug { get; set; }

    protected async override Task OnInitializedAsync()
    {
        StateContainer.OnChange += StateHasChangedAsync;

        var userId = AuthenticatedUser.GetUserId();
        var storageId = await BrowserStorageService.GetOrAddStorageId();
        var response = await Mediator.Send(
            new GetGameRequest(GameId, storageId, userId));

        if (response.Success)
        {
            await Initialize(response.Game!);
        }
        else
        {
            Navigation.NavigateTo("/");
        }

    }

    async Task Initialize(Game game)
    {
        await StateContainer.SetGame(game);

        if (game.ColourScheme == ColourScheme.Alternate)
        {
            await JSRuntime.InvokeVoidAsync("App.setAlternateColourScheme");
        }
        else
        {
            await JSRuntime.InvokeVoidAsync("App.setDefaultColourScheme");
        }

        if (game.AtomShape == AtomShape.Varied)
        {
            await JSRuntime.InvokeVoidAsync("App.setVariedAtomShape");
        }
        else
        {
            await JSRuntime.InvokeVoidAsync("App.setDefaultAtomShape");
        }

        await JSRuntime.InvokeVoidAsync("App.startMusic");
    }

    protected void OnGoToHomePage()
    {
        Navigation.NavigateTo("/");
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