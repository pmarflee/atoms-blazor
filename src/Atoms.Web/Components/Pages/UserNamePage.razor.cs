namespace Atoms.Web.Components.Pages;

public class UserNamePageComponent : Component2Base
{
    [Inject]
    NavigationManager Navigation { get; set; } = default!;

    protected void NameChanged()
    {
        Navigation.NavigateTo("/", true);
    }
}
