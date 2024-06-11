using IdeaManagement.Shared.Entities;

namespace IdeaManagement.API.Repositories;

public interface ICategoryRepository
{
    public Task CreateCategory(string title);
    public List<Category> GetAllCategories();
    public Category FindCategoryById(string categoryId);
    public Task DeleteCategoryById(string categoryId);
    public Task UpdateCategoryTitleById(string categoryId, string newTitle);
}