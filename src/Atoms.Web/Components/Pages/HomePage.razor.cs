﻿using Game = Atoms.Core.Entities.Game;

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

    protected void OnCreateGame(Game game)
    {
        var firstHumanPlayer = game.Players.FirstOrDefault(p => p.IsHuman);

        if (firstHumanPlayer is not null 
            && string.IsNullOrEmpty(firstHumanPlayer.Name))
        {
            Navigation.NavigateToSetUserNamePage(game);
        }
        else
        {
            Navigation.NavigateToGame(game);
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