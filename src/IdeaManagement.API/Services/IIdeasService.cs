using IdeaManagement.Shared.DTOs;
using IdeaManagement.Shared.Entities;

namespace IdeaManagement.API.Services;

public interface IIdeasService
{
    public Task CreateIdea(string title, string? description, string authorId, string authorHandle, string categoryId);
    public List<DTOs.IdeaSlim> GetAllIdeas();
}