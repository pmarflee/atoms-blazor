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

    public UserId? UserId => AuthenticatedUser?.GetUserId();

    public ValueTask<StorageId> GetOrAddStorageId() => BrowserStorageService.GetOrAddStorageId();

    public async Task<string?> GetUserName() =>
        AuthenticatedUser?.GetUserName()
        ?? (await BrowserStorageService.GetUserName());

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
