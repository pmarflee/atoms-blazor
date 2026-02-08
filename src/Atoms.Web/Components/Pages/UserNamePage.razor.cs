using Atoms.UseCases.SetUserName;

namespace Atoms.Web.Components.Pages;

public class UserNamePageComponent : Component2Base
{
    [Inject]
    VisitorIdCookieValueService CookieValueService { get; set; } = default!;

    [Inject]
    NavigationManager NavigationManager { get; set; } = default!;

    [CascadingParameter]
    public HttpContext HttpContext { get; set; } = default!;

    protected async Task HandleValidUserName(UsernameDTO username)
    {
        await Mediator.Send(
            new SetUserNameRequest(
                VisitorId, new UserIdentity(username.Name)));

        CookieValueService.SetName(HttpContext, username.Name!);

        NavigationManager.NavigateTo("/");
    }
}
