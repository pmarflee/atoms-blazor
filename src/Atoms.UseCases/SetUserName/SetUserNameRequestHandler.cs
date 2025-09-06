
namespace Atoms.UseCases.SetUserName;

public class SetUserNameRequestHandler(
    IDbContextFactory<ApplicationDbContext> dbContextFactory,
    ILocalStorageUserService localStorageUserService) 
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

        var localStorageId = await localStorageUserService.GetOrAddLocalStorageId(
            cancellationToken);

        var localStorageUser = dbContext.Find<LocalStorageUserDTO>(localStorageId.Value)!;

        localStorageUser.Name = userName;

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
