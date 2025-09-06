using Atoms.UseCases.CreateNewGame;
using Atoms.UseCases.Menu.GameOptions;

namespace Atoms.Web.Components.Shared;

public partial class MenuComponent : Component2Base
{
    [Inject]
    protected NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    protected IInviteSerializer InviteSerializer { get; set; } = default!;

    [Parameter]
    public EventCallback<Game> OnCreateGame { get; set; }

    protected MenuState State { get; set; }
    protected GameMenuOptions Options { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        var response = await Mediator.Send(
            new CreateGameOptionsRequest(
                Core.Constants.MaxPlayers,
                await GetOrAddStorageId(),
                UserId));

        Options = response.Options;
        State = MenuState.Menu;

        await SetDisplayColourScheme(Options.ColourScheme);
    }

    protected async Task SubmitAsync()
    {
        var response = await Mediator.Send(
            new CreateNewGameRequest(
                Guid.NewGuid(),
                Options,
                new(UserId, await GetUserName())));

        await OnCreateGame.InvokeAsync(response.Game);
    }

    protected void ShowAbout()
    {
        State = MenuState.About;
    }

    protected void HideAbout()
    {
        State = MenuState.Menu;
    }

    protected async Task CopyInviteToClipboard(Uri url)
    {
        await JSRuntime.InvokeVoidAsync("App.copyToClipboard", url.ToString());
    }

    protected async Task ColourSchemeChanged(int value)
    {
        Options.ColourScheme = ColourScheme.FromValue(value);

        await SaveGameMenuOptions();
        await SetDisplayColourScheme(Options.ColourScheme);
    }

    protected async Task AtomShapeChanged(int value)
    {
        Options.AtomShape = AtomShape.FromValue(value);

        await SaveGameMenuOptions();
    }

    protected async Task SoundChanged(bool hasSound)
    {
        Options.HasSound = hasSound;

        if (!hasSound)
        {
            await JSRuntime.InvokeVoidAsync("App.stopMusic");
        }

        await SaveGameMenuOptions();
    }

    protected async Task NumberOfPlayersChanged(int numberOfPlayers)
    {
        Options.NumberOfPlayers = numberOfPlayers;

        await SaveGameMenuOptions();
    }

    protected async Task PlayerTypeChanged(
        GameMenuOptions.Player player, int playerTypeValue)
    {
        player.Type = PlayerType.FromValue(playerTypeValue);

        await SaveGameMenuOptions();
    }

    async Task SaveGameMenuOptions()
    {
        await BrowserStorageService.SetGameMenuOptions(Options);
    }

    protected IEnumerable<PlayerType> PlayerTypes => PlayerType.List.OrderBy(x => x.Value);
    protected IEnumerable<ColourScheme> ColourSchemes => ColourScheme.List.OrderBy(x => x.Value);
    protected IEnumerable<AtomShape> AtomShapes => AtomShape.List.OrderBy(x => x.Value);
}
