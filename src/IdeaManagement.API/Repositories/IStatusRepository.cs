using IdeaManagement.Shared.Entities;

namespace IdeaManagement.API.Repositories;

public interface IStatusRepository
{
    public Task CreateStatus(string title);
    public Status GetStatusByTitle(string title);
    public Status GetStatusById(string statusId);
    public List<Status> GetAllStatuses();
    public Task DeleteStatusById(string statusId);
    public Task UpdateStatusTitleById(string statusId, string newTitle);
}