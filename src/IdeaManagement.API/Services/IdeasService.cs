using IdeaManagement.API.Repositories;
using IdeaManagement.Shared;
using IdeaManagement.Shared.DTOs;
using IdeaManagement.Shared.Entities;
using IdeaManagement.Shared.Exceptions;

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

    public DTOs.IdeaFull GetIdeaDetails(string ideaId) => ideasRepository.GetIdeaDetails(ideaId);

    public async Task DeleteIdea(string ideaId, string userId)
    {
        var idea = ideasRepository.GetIdeaDetails(ideaId);

        if (userId != idea.Author.Key)
            throw new UnauthorizedAccessException("The current user is unable to delete this idea");
        
        await ideasRepository.DeleteIdea(ideaId);
    }

    public async Task UpdateIdeaTitle(string ideaId, string userId, string newTitle)
    {
        var idea = ideasRepository.GetIdeaDetails(ideaId);

        if (userId != idea.Author.Key)
            throw new UnauthorizedAccessException("The current user is unable to edit this idea");
        
        await ideasRepository.UpdateIdeaTitle(ideaId, newTitle);
    }

    public async Task UpdateIdeaDescription(string ideaId, string userId, string newDescription)
    {
        var idea = ideasRepository.GetIdeaDetails(ideaId);

        if (userId != idea.Author.Key)
            throw new UnauthorizedAccessException("The current user is unable to edit this idea");
        
        await ideasRepository.UpdateIdeaDescription(ideaId, newDescription);
    }

    public async Task UpdateIdeaStatus(string ideaId, string userId, string cmdStatusId)
    {
        var status = statusRepository.GetStatusById(cmdStatusId);

        if (status == null)
            throw new DatabaseExceptions.DocumentNotFoundException("Status not found");

        await ideasRepository.UpdateIdeaStatus(ideaId, cmdStatusId);
    }

    public async Task UpdateIdeaCategory(string ideaId, string userId, string cmdCategoryId)
    {
        var category = categoryRepository.FindCategoryById(cmdCategoryId);

        if (category == null)
            throw new DatabaseExceptions.DocumentNotFoundException("Status not found");

        await ideasRepository.UpdateIdeaCategory(ideaId, cmdCategoryId);
    }
}