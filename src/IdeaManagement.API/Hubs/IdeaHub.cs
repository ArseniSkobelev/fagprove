using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace IdeaManagement.API.Hubs;

[Authorize]
public class IdeaHub : Hub
{
}