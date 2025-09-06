namespace Atoms.UseCases.GetLocalStorageUserName;

public record GetLocalStorageUserNameRequest(StorageId LocalStorageId) 
    : IRequest<string?>;
