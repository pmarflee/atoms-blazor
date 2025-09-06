namespace Atoms.UseCases.GetOrAddLocalStorageId;

public class GetOrAddLocalStorageIdRequestHandler(
    ILocalStorageUserService localStorageUserService)
    : IRequestHandler<GetOrAddLocalStorageIdRequest, StorageId>
{
    public async Task<StorageId> Handle(GetOrAddLocalStorageIdRequest _,
                                        CancellationToken cancellationToken)
    {
        return await localStorageUserService.GetOrAddLocalStorageId(
            cancellationToken);
    }
}
