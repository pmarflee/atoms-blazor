using Atoms.UseCases.Invites.AcceptInvite;
using Atoms.UseCases.Invites.ReadInviteCode;

namespace Atoms.Web.Components.Pages;

public class InvitePageComponent : Component2Base
{
    protected string? ErrorMessage;
    protected bool ShowForm;

    Invite _invite = default!;
    UserId? _userId;

    [Inject]
    NavigationManager Navigation { get; set; } = default!;

    [Inject]
    VisitorIdCookieValueService CookieValueService { get; set; } = default!;

    [CascadingParameter]
    public HttpContext HttpContext { get; set; } = default!;

    [Parameter]
    public string Code { get; set; } = default!;

    protected async override Task OnInitializedAsync()
    {
        var response = await Mediator.Send(new ReadInviteCodeRequest(Code));

        if (response.IsSuccessful)
        {
            _invite = response.Invite!;
            _userId = UserId;

            var username = UserName;

            if (!string.IsNullOrEmpty(username))
            {
                await AcceptInvite(new UsernameDTO { Name = username });
            }
            else
            {
                ShowForm = true;
            }
        }
        else
        {
            ErrorMessage = response.ErrorMessage;
        }
    }

    protected async Task AcceptInvite(UsernameDTO username)
    {
        var response = await Mediator.Send(
            new AcceptInviteRequest(
                _invite, VisitorId, new(_userId, username.Name)));

        if (response.Success)
        {
            CookieValueService.SetName(HttpContext, username.Name!);
            Navigation.NavigateToGame(response.GameId!.Value);
        }
        else
        {
            ErrorMessage = response.ErrorMessage;
        }
    }
}
