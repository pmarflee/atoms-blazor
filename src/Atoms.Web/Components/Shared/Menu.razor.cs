using Atoms.UseCases.CreateNewGame;
using Atoms.UseCases.Menu.GameOptions;

namespace Atoms.Web.Components.Shared;

public partial class MenuComponent : Component2Base
{
    [Inject]
    protected NavigationManager NavigationManager { get; set; } = default!;

    [Parameter]
    public EventCallback<GameDTO> OnCreateGame { get; set; }

    protected GameMenuOptions Options { get; set; } = GameMenuOptions.Default;

    protected override async Task OnInitializedAsync()
    {
    }

    protected async override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var response = await Mediator.Send(
                new CreateGameOptionsRequest(
                    Core.Constants.MaxPlayers,
                    VisitorId,
                    UserId));

            Options = response.Options;

            await SetDisplayColourScheme(Options.ColourScheme);

            StateHasChanged();
        }
    }

    protected async Task SubmitAsync()
    {
        var response = await Mediator.Send(
            new CreateNewGameRequest(
                Options, VisitorId, new(UserId, UserName)));

        await OnCreateGame.InvokeAsync(response.Game);
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
