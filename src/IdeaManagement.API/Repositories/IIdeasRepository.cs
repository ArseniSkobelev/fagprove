using IdeaManagement.Shared.DTOs;
using IdeaManagement.Shared.Entities;

namespace IdeaManagement.API.Repositories;

public interface IIdeasRepository
{
    public List<DTOs.IdeaSlim> GetAllIdeas();
    public DTOs.IdeaFull GetIdeaDetails(string ideaId);
    public Task<Idea> CreateIdea(string title, string? description, string authorId, string authorHandle, string statusId, string categoryId);
    public Task UpvoteIdea(string ideaId, string userId);
    public Task RemoveVote(string ideaId, string userId);
    public List<string> GetUpvotedUserIdeas(string userId);
    public Task DeleteIdea(string ideaId);
    public Task UpdateIdeaTitle(string ideaId, string newTitle);
    public Task UpdateIdeaDescription(string ideaId, string newDescription);
    public Task UpdateIdeaStatus(string ideaId, string newStatusId);
    public Task UpdateIdeaCategory(string ideaId, string newCategoryId);
}