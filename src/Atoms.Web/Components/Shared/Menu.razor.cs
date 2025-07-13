using Atoms.UseCases.CreateNewGame;
using Atoms.UseCases.Menu.GameOptions;

namespace Atoms.Web.Components.Shared;

public partial class MenuComponent : Component2Base
{
    protected const int MinPlayers = 2;
    protected const int MaxPlayers = 4;

    [Inject]
    IBrowserStorageService BrowserStorageService { get; set; } = default!;

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
                Guid.NewGuid(),
                MaxPlayers,
                await BrowserStorageService.GetOrAddStorageId(),
                AuthenticatedUser.GetUserId()));

        Options = response.Options;
        State = MenuState.Menu;

        await SetDisplayColourScheme(Options.ColourScheme);
    }

    protected async Task SubmitAsync()
    {
        var response = await Mediator.Send(
            new CreateNewGameRequest(
                Options,
                new(AuthenticatedUser.GetUserId(), await GetUserName())));

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

        await BrowserStorageService.SetColourScheme(Options.ColourScheme);
        await SetDisplayColourScheme(Options.ColourScheme);
    }

    protected async Task AtomShapeChanged(int value)
    {
        Options.AtomShape = AtomShape.FromValue(value);

        await BrowserStorageService.SetAtomShape(Options.AtomShape);
    }

    protected async Task SoundChanged(bool hasSound)
    {
        Options.HasSound = hasSound;

        if (!hasSound)
        {
            await JSRuntime.InvokeVoidAsync("App.stopMusic");
        }

        await BrowserStorageService.SetSound(hasSound);
    }

    async Task<string?> GetUserName()
    {
        return AuthenticatedUser.GetUserName() 
            ?? (await BrowserStorageService.GetUserName());
    }
}
