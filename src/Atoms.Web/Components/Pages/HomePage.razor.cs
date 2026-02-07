namespace Atoms.Web.Components.Pages;

public partial class HomePageComponent : Component2Base, IDisposable
{
    [Inject]
    NavigationManager Navigation { get; set; } = default!;

    [Inject]
    GameStateContainer StateContainer { get; set; } = default!;

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

    protected void OnCreateGame(GameDTO gameDto)
    {
        var firstHumanPlayer = gameDto.Players.FirstOrDefault(
            p => p.PlayerTypeId == PlayerType.Human);

        if (firstHumanPlayer is not null 
            && string.IsNullOrEmpty(firstHumanPlayer.AbbreviatedName))
        {
            Navigation.NavigateToSetUserNamePage(gameDto);
        }
        else
        {
            Navigation.NavigateToGame(gameDto);
        }
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