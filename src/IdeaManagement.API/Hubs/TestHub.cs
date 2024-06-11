using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace IdeaManagement.API.Hubs;

[Authorize]
public class TestHub(IHubContext<TestHub> context) : Hub
{
    public async Task SendMessage(string message)
    {
        await context.Clients.All.SendAsync("SendMessage", message);
    }
}