using Atoms.UseCases.GetLocalStorageUserName;
using Atoms.UseCases.GetOrAddLocalStorageId;

namespace Atoms.Web.Components;

public abstract class Component2Base : ComponentBase
{
    [Inject]
    public IMediator Mediator { get; set; } = default!;

    [Inject]
    public IJSRuntime JSRuntime { get; set; } = default!;

    [Inject]
    public IBrowserStorageService BrowserStorageService { get; set; } = default!;

    [CascadingParameter]
    public ClaimsPrincipal? AuthenticatedUser { get; set; }

    [CascadingParameter]
    public VisitorId VisitorId { get; private set; } = default!;

    public UserId? UserId => AuthenticatedUser?.GetUserId();

    public Task<StorageId> GetOrAddStorageId() => Mediator.Send(new GetOrAddLocalStorageIdRequest());

    public async Task<string?> GetUserName() =>
        AuthenticatedUser?.GetUserName()
        ?? (await GetUserNameForLocalStorageId());

    public async Task<UserIdentity> GetUserIdentity()
    {
        return new(UserId, await GetUserName());
    }

    protected async Task SetDisplayColourScheme(ColourScheme colourScheme)
    {
        var colourSchemeFuncName = colourScheme == ColourScheme.Alternate
            ? "Alternate"
            : "Default";

        await JSRuntime.InvokeVoidAsync($"App.set{colourSchemeFuncName}ColourScheme");
    }

    protected async Task SetDisplayAtomShape(AtomShape atomShape)
    {
        var atomShapeFuncName = atomShape == AtomShape.Varied
            ? "Varied"
            : "Default";

        await JSRuntime.InvokeVoidAsync($"App.set{atomShapeFuncName}AtomShape");
    }

    async Task<string?> GetUserNameForLocalStorageId()
    {
        var localStorageId = await GetOrAddStorageId();

        return await Mediator.Send(
            new GetLocalStorageUserNameRequest(localStorageId));
    }
}
