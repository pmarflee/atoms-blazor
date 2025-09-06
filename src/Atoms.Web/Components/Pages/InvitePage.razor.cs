using Atoms.UseCases.Invites.AcceptInvite;
using Atoms.UseCases.Invites.ReadInviteCode;

namespace Atoms.Web.Components.Pages;

public class InvitePageComponent : Component2Base
{
    protected string? ErrorMessage;
    protected bool InviteCodeRead;

    Invite _invite = default!;
    UserId? _userId;

    [Inject]
    NavigationManager Navigation { get; set; } = default!;

    [Parameter]
    public string Code { get; set; } = default!;

    protected InputText InputName { get; set; } = default!;

    protected InputModel Input { get; set; } = new();

    protected async override Task OnInitializedAsync()
    {
        var response = await Mediator.Send(new ReadInviteCodeRequest(Code));

        InviteCodeRead = response.IsSuccessful;

        if (response.IsSuccessful)
        {
            _invite = response.Invite!;
            _userId = UserId;

            var username = await GetUserName();

            if (!string.IsNullOrEmpty(username))
            {
                Input.Name = username;

                await AcceptInvite();
            }
        }
        else
        {
            ErrorMessage = response.ErrorMessage;
        }
    }

    protected async override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && InputName?.Element is not null)
        {
            await InputName.Element.Value.FocusAsync();
        }
    }

    public async Task AcceptInvite()
    {
        var response = await Mediator.Send(
            new AcceptInviteRequest(_invite, new(_userId, Input.Name)));

        if (response.Success)
        {
            Navigation.NavigateToGame(_invite);
        }
        else
        {
            ErrorMessage = response.ErrorMessage;
        }
    }

    protected sealed class InputModel
    {
        [Required, MaxLength(25), RegularExpression("[A-Za-z0-9_ ]+")]
        public string Name { get; set; } = default!;
    }
}
