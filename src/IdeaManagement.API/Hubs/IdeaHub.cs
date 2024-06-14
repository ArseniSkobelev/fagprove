using System.Security.Claims;
using IdeaManagement.API.Domain.SignalR;
using IdeaManagement.API.Services;
using IdeaManagement.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace IdeaManagement.API.Hubs;

[Authorize]
public class IdeaHub(IHubContext<IdeaHub> context, UserRoleMapping userRoleMapping, IAuth0Service auth0Service, IIdeasService ideasService, ICategoryService categoryService) : Hub<IIdeaHubClient>
{
    private static readonly HubConnectionMapping<string> _connections = new();

    public async Task NotifyNewIdeaAdded(string authorId, string ideaId, string categoryId)
    {
        var author = await auth0Service.GetUserById(authorId);
        var idea = ideasService.GetIdeaDetails(ideaId);
        var categories = categoryService.GetAllCategories();
        var category = categories.FirstOrDefault(x => x.Id == categoryId);

        if (category == null || string.IsNullOrWhiteSpace(category.OwnerId))
        {
            await context.Clients.All.SendAsync(SignalR.Methods.NewIdeaAdded, author.Name, idea.Title);
            return;
        }

        foreach (var connectionId in _connections.GetConnections(category.OwnerId))
        {
            await context.Clients.Client(connectionId).SendAsync(SignalR.Methods.NewIdeaAdded, author.Name, idea.Title);
        }
    }

    public override async Task OnConnectedAsync()
    {
        if (Context.User == null)
        {
            await base.OnConnectedAsync();
            return;
        }

        var userIdMaybe = Context.User.Claims.FirstOrDefault(x => x.Type == "sub");

        if (userIdMaybe == null)
        {
            await base.OnConnectedAsync();
            return;
        }

        var userRoleMaybe = Context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role);

        if (userRoleMaybe != null)
        {
            userRoleMapping.UsersWithRoles.TryAdd(userIdMaybe.Value, userRoleMaybe.Value);
        }

        _connections.Add(userIdMaybe.Value, Context.ConnectionId);

        await base.OnConnectedAsync();
    }
}