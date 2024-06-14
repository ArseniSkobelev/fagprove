using IdeaManagement.Shared.DTOs;
using IdeaManagement.Shared.Entities;

namespace IdeaManagement.API.Services;

public interface ICategoryService
{
    public Task CreateCategory(string title);
    public List<Category> GetAllCategories();
    public Category FindCategoryById(string categoryId);
    public Task DeleteCategoryById(string categoryId);
    public Task UpdateCategoryTitleById(string categoryId, string newTitle);
    public Task UpdateCategoryOwner(string categoryId, string ownerId);
    public List<Category> GetUserOwnedCategories(string userId);
}