using IdeaManagement.API.Hubs;
using IdeaManagement.API.Repositories;
using Microsoft.AspNetCore.SignalR;

namespace IdeaManagement.API.Services;

class NotificationService(IHubContext<IdeaHub> ideaHubContext, IAuth0Service auth0Service, IIdeasService ideasService) : INotificationService
{
    public async Task NotifyNewIdeaAdded(string authorId, string ideaId, string categoryTitle)
    {
        var ideaAuthor = await auth0Service.GetUserById(authorId);
        var idea = ideasService.GetIdeaDetails(ideaId);

        throw new NotImplementedException();
    }

    public Task NotifyNewCommentAdded(string authorId, string ideaId)
    {
        throw new NotImplementedException();
    }

    public Task NotifyIdeaStatusChanged(string ideaId, string newStatusId)
    {
        throw new NotImplementedException();
    }
}