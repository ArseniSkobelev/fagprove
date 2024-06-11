using IdeaManagement.Shared.DTOs;
using IdeaManagement.Shared.Entities;

namespace IdeaManagement.API.Repositories;

public interface IIdeasRepository
{
    public List<DTOs.IdeaSlim> GetAllIdeas();
    public Task<Idea> GetIdeaDetails(string ideaId);
    public Task CreateIdea(string title, string? description, string authorId, string authorHandle, string statusId, string categoryId);
    public Task UpvoteIdea(string ideaId, string userId);
    public Task RemoveVote(string ideaId, string userId);
    public List<string> GetUpvotedUserIdeas(string userId);
}