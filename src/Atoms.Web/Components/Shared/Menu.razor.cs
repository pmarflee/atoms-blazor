using Atoms.UseCases.CreateNewGame;
using Atoms.UseCases.Menu.GameOptions;

namespace Atoms.Web.Components.Shared;

public partial class MenuComponent : Component2Base
{
    [Inject]
    BrowserStorageService BrowserStorageService { get; set; } = default!;

    [Inject]
    NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    IJSRuntime JSRuntime { get; set; } = default!;

    [Parameter]
    public EventCallback<Game> OnCreateGame { get; set; }

    protected MenuState State { get; set; }
    protected GameMenuOptions Options { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        var response = await Mediator.Send(
            new CreateGameOptionsRequest(NavigationManager.BaseUri));

        Options = response.Options;
        State = MenuState.Menu;
    }

    protected async Task SubmitAsync()
    {
        var response = await Mediator.Send(
            new CreateNewGameRequest(
                Options, await BrowserStorageService.GetOrAddStorageId()));

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

    protected async Task CopyInviteToClipboard(InviteLink link)
    {
        await JSRuntime.InvokeVoidAsync("App.copyToClipboard", link.Url);
    }
}
