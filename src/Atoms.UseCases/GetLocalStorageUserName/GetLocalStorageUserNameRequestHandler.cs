
namespace Atoms.UseCases.GetLocalStorageUserName;

public class GetLocalStorageUserNameRequestHandler(
    IDbContextFactory<ApplicationDbContext> dbContextFactory)
    : IRequestHandler<GetLocalStorageUserNameRequest, string?>
{
    public async Task<string?> Handle(GetLocalStorageUserNameRequest request,
                                CancellationToken cancellationToken)
    {
        using var dbContext = await dbContextFactory.CreateDbContextAsync(
            cancellationToken);

        var localStorageUser = await dbContext.FindAsync<LocalStorageUserDTO>(
            request.LocalStorageId.Value);

        return localStorageUser?.Name;
    }
}
