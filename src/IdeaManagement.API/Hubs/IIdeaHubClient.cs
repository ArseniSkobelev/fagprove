namespace IdeaManagement.API.Hubs;

public interface IIdeaHubClient
{
    public Task NotifyNewIdeaAdded(string authorId, string ideaId, string categoryId);
    public Task NotifyIdeasUpdated();
}