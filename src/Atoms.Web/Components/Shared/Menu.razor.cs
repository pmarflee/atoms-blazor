﻿using Atoms.Core.Enums;
using Atoms.UseCases.CreateNewGame;
using MediatR;

namespace Atoms.Web.Components.Shared;

public partial class MenuComponent : ComponentBase
{
    [Parameter]
    public EventCallback<Game> OnCreateGame { get; set; }

    [Inject]
    public IMediator Mediator { get; set; } = default!;

    protected MenuState State { get; set; }
    protected GameMenuOptions Options { get; set; } = default!;

    protected override void OnInitialized()
    {
        Options = new(GameMenuOptions.MinPlayers, GameMenuOptions.MaxPlayers);
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
}
