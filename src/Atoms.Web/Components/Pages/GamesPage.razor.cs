using Atoms.Core.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.FluentUI.AspNetCore.Components;

namespace Atoms.Web.Components.Pages;

public class GamesPageComponent : Component2Base, IAsyncDisposable
{
    [Inject]
    protected IDbContextFactory<ApplicationDbContext> DbContextFactory { get; set; } = default!;

    [Inject]
    NavigationManager Navigation { get; set; } = default!;

    protected IQueryable<GameInfoDTO> Games { get; set; } = default!;
    protected IQueryable<GameInfoDTO>? ActiveGames => Games?
        .Where(game => game.IsActive);
    protected IQueryable<GameInfoDTO>? InactiveGames => Games?
        .Where(game => !game.IsActive);

    protected PaginationState ActiveGamesPagination = new() { ItemsPerPage = 8 };
    protected PaginationState InactiveGamesPagination = new() { ItemsPerPage = 8 };

    protected string ActiveTabId { get; set; } = default!;

    ApplicationDbContext? _dbContext;

    protected override async Task OnInitializedAsync()
    {
        _dbContext = await DbContextFactory.CreateDbContextAsync();

        Games = _dbContext.GetGamesForUser(await GetOrAddStorageId(), UserId);
    }

    protected void OpenGame(GameInfoDTO game)
    {
        Navigation.NavigateToGame(game);
    }

    public async ValueTask DisposeAsync()
    {
        if (_dbContext is not null)
        {
            await _dbContext.DisposeAsync();
        }

        GC.SuppressFinalize(this);
    }
}
