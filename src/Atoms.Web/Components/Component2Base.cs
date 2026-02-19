using Atoms.Core;

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

    [CascadingParameter(Name = Constants.CascadingValues.VisitorUserName)]
    public string? VisitorUserName { get; init; }

    public UserId? UserId => AuthenticatedUser?.GetUserId();

    public string? UserName
    {
        get
        {
            return VisitorUserName
                ?? AuthenticatedUser?.FindFirstValue(Constants.Claims.Name);
        }
    }

    public async Task<UserIdentity> GetUserIdentity() => new(UserId, UserName);

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
}
