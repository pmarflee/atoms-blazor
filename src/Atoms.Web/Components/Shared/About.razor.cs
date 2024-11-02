namespace Atoms.Web.Components.Shared;

public partial class AboutComponent : ComponentBase
{
    [Parameter]
    public EventCallback<Game> OnHide { get; set; }

    protected async Task HideAboutAsync()
    {
        await OnHide.InvokeAsync();
    }
}
