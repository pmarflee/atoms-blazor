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

        using var dbContext = await dbContextFactory.CreateDbContextAsync(
            cancellationToken);

        var gameDto = await dbContext.GetGameById(request.GameId,
                                                  cancellationToken) 
            ?? throw new Exception("Game not found");

        var newGame = await gameCreationService.CreateGame(
            gameDto.CreateOptionsForRematch(hasSound),
            request.VisitorId,
            request.UserIdentity,
            cancellationToken);

        if (request.OpponentConnectionIds?.Count > 0)
        {
            await NotifyOpponents(request.UserIdentity,
                                  request.OpponentConnectionIds,
                                  request.VisitorId,
                                  newGame,
                                  cancellationToken);
        }

        return new(newGame);
    }

    private async Task NotifyOpponents(UserIdentity userIdentity,
                                       List<string> connectionIds,
                                       VisitorId visitorId,
                                       GameDTO gameDto,
                                       CancellationToken cancellationToken)
    {
        var challengePlayer = gameDto.Players
            .Where(player => gameDto.PlayerBelongsToUser(
                player,
                userIdentity.Id,
                visitorId))
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
