
namespace Atoms.UseCases.SetUserName;

public class SetUserNameRequestHandler(
    IDbContextFactory<ApplicationDbContext> dbContextFactory,
    IVisitorService visitorService) 
    : IRequestHandler<SetUserNameRequest>
{
    public async Task Handle(SetUserNameRequest request,
                             CancellationToken cancellationToken)
    {
        var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        var userName = request.UserIdentity.Name!;

        if (request.Game is not null)
        {
            var gameDto = await dbContext.GetGameById(request.Game.Id, cancellationToken);
            var playerDto = gameDto!.Players
                .OrderBy(p => p.Number)
                .First(p => p.PlayerTypeId == PlayerType.Human);

            if (string.IsNullOrEmpty(playerDto.AbbreviatedName))
            {
                playerDto.AbbreviatedName = request.UserIdentity.GetAbbreviatedName();
            }
        }

        await visitorService.AddOrUpdate(
            request.VisitorId,
            cancellationToken);

        var visitor = dbContext.Find<VisitorDTO>(request.VisitorId.Value)!;

        visitor.Name = userName;

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
