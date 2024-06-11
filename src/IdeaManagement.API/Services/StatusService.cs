using IdeaManagement.API.Repositories;
using IdeaManagement.Shared.Entities;

namespace IdeaManagement.API.Services;

public class StatusService(IStatusRepository statusRepository) : IStatusService
{
    public async Task CreateStatus(string title) =>
        await statusRepository.CreateStatus(title);

    public List<Status> GetAllStatuses() =>
        statusRepository.GetAllStatuses();

    public Status FindStatusById(string statusId) =>
        statusRepository.GetStatusById(statusId);

    public async Task DeleteStatusById(string statusId) =>
        await statusRepository.DeleteStatusById(statusId);

    public async Task UpdateStatusTitleById(string statusId, string newTitle) =>
        await statusRepository.UpdateStatusTitleById(statusId, newTitle);
}