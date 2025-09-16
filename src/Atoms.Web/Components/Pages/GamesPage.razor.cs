using Atoms.Core.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.FluentUI.AspNetCore.Components;

namespace Atoms.Web.Components.Pages;

public class GamesPageComponent : Component2Base, IAsyncDisposable
{
    [Inject]
    protected IDbContextFactory<ApplicationDbContext> DbContextFactory { get; set; } = default!;

    [Inject]
    protected ILocalStorageUserService LocalStorageUserService { get; set; } = default!;

    [Inject]
    NavigationManager Navigation { get; set; } = default!;

    protected IQueryable<GameInfoDTO>? ActiveGames;
    protected IQueryable<GameInfoDTO>? InactiveGames;

    protected PaginationState ActiveGamesPagination = new() { ItemsPerPage = 8 };
    protected PaginationState InactiveGamesPagination = new() { ItemsPerPage = 8 };

    protected string ActiveTabId { get; set; } = default!;

    ApplicationDbContext? _dbContextActiveGames;
    ApplicationDbContext? _dbContextInactiveGames;

    protected override async Task OnInitializedAsync()
    {
        _dbContextActiveGames = await DbContextFactory.CreateDbContextAsync();
        _dbContextInactiveGames = await DbContextFactory.CreateDbContextAsync();

        var localStorageId = await LocalStorageUserService.GetOrAddLocalStorageId(_dbContextActiveGames);

        ActiveGames = _dbContextActiveGames
            .GetGamesForUser(localStorageId, UserId)
            .Where(game => game.IsActive);

        InactiveGames = _dbContextInactiveGames
            .GetGamesForUser(localStorageId, UserId)
            .Where(game => !game.IsActive);
    }

    protected void OpenGame(GameInfoDTO game)
    {
        Navigation.NavigateToGame(game);
    }

    public async ValueTask DisposeAsync()
    {
        if (_dbContextActiveGames is not null)
        {
            await _dbContextActiveGames.DisposeAsync();
        }

        if (_dbContextInactiveGames is not null)
        {
            await _dbContextInactiveGames.DisposeAsync();
        }

        GC.SuppressFinalize(this);
    }
}
