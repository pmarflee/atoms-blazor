
namespace Atoms.UseCases.SetUserName;

public class SetUserNameRequestHandler(
    IDbContextFactory<ApplicationDbContext> dbContextFactory) 
    : IRequestHandler<SetUserNameRequest>
{
    public async Task Handle(SetUserNameRequest request,
                             CancellationToken cancellationToken)
    {
        var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        var gameDto = await dbContext.GetGameById(request.Game.Id, cancellationToken);
        var playerDto = gameDto!.Players
            .OrderBy(p => p.Number)
            .First(p => p.Type == PlayerType.Human);

        if (!string.IsNullOrEmpty(playerDto.Name)) return;

        var userName = request.UserIdentity.Name!;

        playerDto.Name = userName;
        playerDto.AbbreviatedName = request.UserIdentity.GetAbbreviatedName();

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
