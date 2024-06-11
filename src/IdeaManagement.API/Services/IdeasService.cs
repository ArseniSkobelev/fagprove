using IdeaManagement.API.Repositories;
using IdeaManagement.Shared;
using IdeaManagement.Shared.DTOs;
using IdeaManagement.Shared.Entities;

namespace IdeaManagement.API.Services;

public class IdeasService(IIdeasRepository ideasRepository, IStatusRepository statusRepository, ICategoryRepository categoryRepository) : IIdeasService
{
    public async Task CreateIdea(string title, string? description, string authorId, string authorHandle, string categoryId)
    {
        categoryRepository.FindCategoryById(categoryId);
        
        var newIdeaStatus = statusRepository.GetStatusByTitle(Constants.DefaultStatuses.NewIdea);

        await ideasRepository.CreateIdea(title, description, authorId, authorHandle, newIdeaStatus.Id, categoryId);
    }

    public List<DTOs.IdeaSlim> GetAllIdeas() => ideasRepository.GetAllIdeas();
}