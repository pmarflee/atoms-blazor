using Atoms.Infrastructure.Services;
using Atoms.UseCases.GetGame;
using Atoms.UseCases.SetUserName;

namespace Atoms.Web.Components.Shared;

public class UserNameComponent : Component2Base
{
    [Parameter]
    public string? RedirectUrl { get; set; }

    [Parameter]
    public Guid? GameId { get; set; }

    [Inject]
    VisitorIdCookieValueService CookieValueService { get; set; } = default!;

    [Inject]
    NavigationManager NavigationManager { get; set; } = default!;

    [CascadingParameter]
    public HttpContext HttpContext { get; set; } = default!;

    [SupplyParameterFromForm]
    public UsernameDTO Input { get; set; } = default!;

    protected async override Task OnInitializedAsync()
    {
        Input ??= new UsernameDTO { Name = UserName };
    }

    protected async Task OnValidSubmit()
    {
        Game? game = null;

        if (GameId.HasValue)
        {
            var response = await Mediator.Send(
                new GetGameRequest(GameId.Value, VisitorId, UserId));

            if (response.Success)
            {
                game = response.Game;
            }
            else
            {
                NavigationManager.NavigateTo("/");
            }
        }

        await Mediator.Send(
            new SetUserNameRequest(
                VisitorId, new UserIdentity(Input.Name), game));

        CookieValueService.SetName(HttpContext, Input.Name!);

        if (game is not null)
        {
            NavigationManager.NavigateToGame(game);
        }
        else
        {
            NavigationManager.NavigateTo(RedirectUrl ?? "/");
        }
    }
}
