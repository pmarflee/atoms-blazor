using Atoms.Infrastructure.Identity;

namespace Atoms.Web.Components.Account.Shared;

public partial class StatusMessageComponent : Component2Base
{
    private string? _messageFromCookie;

    [Parameter]
    public string? Message { get; set; }

    [CascadingParameter]
    private HttpContext HttpContext { get; set; } = default!;

    protected string? DisplayMessage => Message ?? _messageFromCookie;

    protected override void OnInitialized()
    {
        _messageFromCookie = HttpContext.Request.Cookies[IdentityRedirectManager.StatusCookieName];

        if (_messageFromCookie is not null)
        {
            HttpContext.Response.Cookies.Delete(IdentityRedirectManager.StatusCookieName);
        }
    }
}