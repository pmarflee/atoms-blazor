namespace Atoms.Web.Components.Shared;

public class UserNameComponent : Component2Base
{
    [Parameter]
    public string? Name { get; set; }

    [Parameter]
    public EventCallback<string?> NameChanged { get; set; }

    protected InputText InputName { get; set; } = default!;

    protected UsernameDTO Input { get; set; } = new();

    protected override void OnInitialized()
    {
        Input.Name = Name;
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
        await NameChanged.InvokeAsync(Input.Name);
    }
}
