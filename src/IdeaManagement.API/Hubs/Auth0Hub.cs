using System.Security.Claims;
using IdeaManagement.API.Domain.SignalR;
using IdeaManagement.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace IdeaManagement.API.Hubs;

[Authorize]
public class Auth0Hub(IHubContext<Auth0Hub> context) : Hub<IAuth0HubClient>
{
    private static readonly HubConnectionMapping<string> _connections = new();

    public async Task UserRoleUpdated(string userId)
    {
        foreach (var connection in _connections.GetConnections(userId))
        {
            await context.Clients.Client(connection).SendAsync(SignalR.Methods.UserRoleUpdated);
        }
    }
    
    public async Task UserBlockStatusUpdated(string userId)
    {
        foreach (var connection in _connections.GetConnections(userId))
        {
            await context.Clients.Client(connection).SendAsync(SignalR.Methods.UserBlockStatusUpdated);
        }
    }

    public override async Task OnConnectedAsync()
    {
        if (Context.User == null)
        {
            await base.OnConnectedAsync();
            return;
        }

        var userIdMaybe = Context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

        if (userIdMaybe == null)
        {
            await base.OnConnectedAsync();
            return;
        }

        _connections.Add(userIdMaybe.Value, Context.ConnectionId);

        await base.OnConnectedAsync();
    }
}