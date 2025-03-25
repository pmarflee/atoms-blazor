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
    IJSRuntime JSRuntime { get; set; } = default!;

    [Inject]
    protected IInviteSerializer InviteSerializer { get; set; } = default!;

    [CascadingParameter]
    ClaimsPrincipal? AuthenticatedUser { get; set; }

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
    }

    protected async Task SubmitAsync()
    {
        var response = await Mediator.Send(new CreateNewGameRequest(Options));

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

    protected async Task ColourSchemeChanged(ColourScheme colourScheme)
    {
        Options.ColourScheme = colourScheme;

        await BrowserStorageService.SetColourScheme(colourScheme);
    }

    protected async Task AtomShapeChanged(AtomShape atomShape)
    {
        Options.AtomShape = atomShape;

        await BrowserStorageService.SetAtomShape(atomShape);
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
}
