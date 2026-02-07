using Atoms.UseCases.GetVisitorUserName;

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
    public VisitorId VisitorId { get; init; } = default!;

    public UserId? UserId => AuthenticatedUser?.GetUserId();

    public async Task<string?> GetUserName() =>
        AuthenticatedUser?.GetUserName()
        ?? (await GetUserNameForVisitorId());

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

    async Task<string?> GetUserNameForVisitorId()
    {
        return await Mediator.Send(new GetVisitorUserNameRequest(VisitorId));
    }
}
