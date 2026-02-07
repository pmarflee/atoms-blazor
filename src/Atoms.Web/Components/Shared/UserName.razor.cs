using Atoms.UseCases.SetUserName;

namespace Atoms.Web.Components.Shared;

public class UserNameComponent : Component2Base
{
    [Parameter]
    public EventCallback<string?> NameChanged { get; set; }

    protected InputText InputName { get; set; } = default!;

    protected UsernameDTO Input { get; set; } = new();

    protected async override Task OnInitializedAsync()
    {
        Input.Name = await GetUserName();
    }

    protected async override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await InputName.Element!.Value.FocusAsync();
        }
    }

    protected async Task OnValidSubmit()
    {
        await Mediator.Send(
            new SetUserNameRequest(
                VisitorId, new UserIdentity(Input.Name)));

        await NameChanged.InvokeAsync(Input.Name);
    }
}
