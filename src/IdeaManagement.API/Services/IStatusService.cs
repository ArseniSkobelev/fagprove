using IdeaManagement.Shared.Entities;

namespace IdeaManagement.API.Services;

public interface IStatusService
{
    public Task CreateStatus(string title);
    public List<Status> GetAllStatuses();
    public Status FindStatusById(string statusId);
    public Task DeleteStatusById(string statusId);
    public Task UpdateStatusTitleById(string statusId, string newTitle);
}