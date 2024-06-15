namespace IdeaManagement.API.Hubs;

public interface IIdeaHubClient
{
    public Task NotifyNewIdeaAdded(string authorId, string ideaId, string categoryId);
    public Task NotifyIdeasUpdated();
    public Task NotifyNewCommentAdded(string ideaId, string ideaTitle);
    public Task NotifyIdeaStatusChanged(string ideaId, string statusTitle, string ideaTitle);
}