using Atoms.UseCases.Invites.AcceptInvite;
using Atoms.UseCases.Invites.ReadInviteCode;

namespace Atoms.Web.Components.Pages;

public class InvitePageComponent : Component2Base
{
    protected string? ErrorMessage;
    protected bool InviteCodeRead;

    Invite _invite = default!;
    UserId? _userId;

    [CascadingParameter]
    ClaimsPrincipal? AuthenticatedUser { get; set; }

    [Inject]
    BrowserStorageService BrowserStorageService { get; set; } = default!;

    [Inject]
    NavigationManager Navigation { get; set; } = default!;

    [Parameter]
    public string Code { get; set; } = default!;

    protected InputModel Input { get; set; } = new();

    protected async override Task OnInitializedAsync()
    {
        var response = await Mediator.Send(new ReadInviteCodeRequest(Code));

        InviteCodeRead = response.IsSuccessful;

        if (response.IsSuccessful)
        {
            _invite = response.Invite!;
            _userId = AuthenticatedUser.GetUserId();

            if (_userId is not null)
            {
                await AcceptInvite();
            }    
        }
        else
        {
            ErrorMessage = "Unable to read invite code";
        }
    }

    public async Task AcceptInvite()
    {
        var storageId = await BrowserStorageService.GetOrAddStorageId();
        var response = await Mediator.Send(
            new AcceptInviteRequest(
                _invite, _userId, storageId, Input.Name));

        if (response.IsSuccessful)
        {
            Navigation.NavigateToGame(_invite);
        }
    }

    protected sealed class InputModel
    {
        [Required]
        public string Name { get; set; } = default!;
    }
}
