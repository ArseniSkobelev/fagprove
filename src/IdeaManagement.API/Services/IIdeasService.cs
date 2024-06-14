using IdeaManagement.Shared.DTOs;
using IdeaManagement.Shared.Entities;

namespace IdeaManagement.API.Services;

public interface IIdeasService
{
    public Task CreateIdea(string title, string? description, string authorId, string authorHandle, string categoryId);
    public List<DTOs.IdeaSlim> GetAllIdeas();
    public DTOs.IdeaFull GetIdeaDetails(string ideaId);
    public Task DeleteIdea(string ideaId, string userId);
    public Task UpdateIdeaTitle(string ideaId, string userId, string newTitle);
    public Task UpdateIdeaDescription(string ideaId, string userId, string newDescription);
    public Task UpdateIdeaStatus(string ideaId, string userId, string cmdStatusId);
    public Task UpdateIdeaCategory(string ideaId, string userId, string cmdCategoryId);
}