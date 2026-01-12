using Atoms.Core.DTOs.Notifications.SignalR;

namespace Atoms.UseCases.CreateRematchGame;

public class CreateRematchGameRequestHandler(
    IBrowserStorageService browserStorageService,
    IDbContextFactory<ApplicationDbContext> dbContextFactory,
    IGameCreationService gameCreationService,
    INotificationService notificationService)
    : IRequestHandler<CreateRematchGameRequest, CreateRematchGameResponse>
{
    public async Task<CreateRematchGameResponse> Handle(
        CreateRematchGameRequest request,
        CancellationToken cancellationToken)
    {
        var hasSound = await browserStorageService.GetSound();
        var localStorageId = await browserStorageService.GetOrAddStorageId();

        using var dbContext = await dbContextFactory.CreateDbContextAsync(
            cancellationToken);

        var gameDto = await dbContext.GetGameById(request.GameId,
                                                  cancellationToken) 
            ?? throw new Exception("Game not found");

        var newGame = await gameCreationService.CreateGame(
            gameDto.CreateOptionsForRematch(hasSound),
            request.UserIdentity,
            cancellationToken);

        if (request.OpponentConnectionIds?.Count > 0)
        {
            await NotifyOpponents(request.UserIdentity,
                                  request.OpponentConnectionIds,
                                  localStorageId,
                                  newGame,
                                  cancellationToken);
        }

        return new(newGame);
    }

    private async Task NotifyOpponents(UserIdentity userIdentity,
                                       List<string> connectionIds,
                                       StorageId localStorageId,
                                       GameDTO gameDto,
                                       CancellationToken cancellationToken)
    {
        var challengePlayer = gameDto.Players
            .Where(player => gameDto.PlayerBelongsToUser(
                player,
                userIdentity.Id,
                localStorageId))
            .OrderByDescending(player => !string.IsNullOrEmpty(player.AbbreviatedName))
            .FirstOrDefault();

        if (challengePlayer is not null)
        {
            Rematch notification = new(
                gameDto.Id,
                connectionIds,
                userIdentity.Name ?? $"Player {challengePlayer.Number}");

            await notificationService.NotifyRematch(
                notification,
                cancellationToken);
        }
    }
}
