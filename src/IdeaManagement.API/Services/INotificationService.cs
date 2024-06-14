namespace IdeaManagement.API.Services;

public interface INotificationService
{
    public Task NotifyNewIdeaAdded(string authorId, string ideaId, string categoryTitle);
    public Task NotifyNewCommentAdded(string authorId, string ideaId);
    public Task NotifyIdeaStatusChanged(string ideaId, string newStatusId);
}