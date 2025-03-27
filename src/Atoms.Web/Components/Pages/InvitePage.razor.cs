using Atoms.UseCases.Invites.AcceptInvite;
using Atoms.UseCases.Invites.ReadInviteCode;
using Microsoft.AspNetCore.SignalR.Client;

namespace Atoms.Web.Components.Pages;

public class InvitePageComponent : Component2Base, IAsyncDisposable
{
    HubConnection? _hubConnection = default!;

    protected string? ErrorMessage;
    protected bool InviteCodeRead;

    Invite _invite = default!;
    UserId? _userId;

    [CascadingParameter]
    ClaimsPrincipal? AuthenticatedUser { get; set; }

    [Inject]
    IBrowserStorageService BrowserStorageService { get; set; } = default!;

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

            _hubConnection = new HubConnectionBuilder()
                        .WithUrl(Navigation.ToAbsoluteUri("/gamehub"))
                        .Build();

            await _hubConnection.StartAsync();

            if (_userId is not null)
            {
                await AcceptInvite();
            }

            var userName = await BrowserStorageService.GetUserName();

            if (!string.IsNullOrEmpty(userName))
            {
                Input.Name = userName;
            }
        }
        else
        {
            ErrorMessage = response.ErrorMessage;
        }
    }

    public async Task AcceptInvite()
    {
        var response = await Mediator.Send(
            new AcceptInviteRequest(_invite, _userId, Input.Name));

        if (response.Success)
        {
            await _hubConnection!.SendAsync(
                "Notify",
                _invite.GameId,
                $"Player {response.Player!.Number} joined");

            Navigation.NavigateToGame(_invite);
        }
        else
        {
            ErrorMessage = response.ErrorMessage;
        }
    }

    protected sealed class InputModel
    {
        [Required]
        public string Name { get; set; } = default!;
    }

    public async ValueTask DisposeAsync()
    {
        if (_hubConnection is not null)
        {
            await _hubConnection.DisposeAsync();
        }

        GC.SuppressFinalize(this);
    }
}
