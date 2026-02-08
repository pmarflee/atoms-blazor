namespace Atoms.Web.Components.Shared;

public class UserNameComponent : Component2Base
{
    [Parameter]
    public EventCallback<UsernameDTO> OnValid { get; set; }

    [SupplyParameterFromForm]
    public UsernameDTO Input { get; set; } = default!;

    protected async override Task OnInitializedAsync()
    {
        Input ??= new UsernameDTO { Name = UserName };
    }

    protected async Task OnValidSubmit()
    {
        await OnValid.InvokeAsync(Input);
    }
}
