namespace Atoms.Web.Components.Shared;

public partial class AboutComponent : Component2Base
{
    [Parameter]
    public EventCallback<Game> OnHide { get; set; }

    protected async Task HideAboutAsync()
    {
        await OnHide.InvokeAsync();
    }
}
