namespace Atoms.UseCases.GetVisitorUserName;

public class GetVisitorUserNameRequestHandler(
    IDbContextFactory<ApplicationDbContext> dbContextFactory)
    : IRequestHandler<GetVisitorUserNameRequest, string?>
{
    public async Task<string?> Handle(GetVisitorUserNameRequest request,
                                CancellationToken cancellationToken)
    {
        using var dbContext = await dbContextFactory.CreateDbContextAsync(
            cancellationToken);

        var visitor = await dbContext.FindAsync<VisitorDTO>(
            request.VisitorId.Value);

        return visitor?.Name;
    }
}
