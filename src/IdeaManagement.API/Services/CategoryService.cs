using IdeaManagement.API.Repositories;
using IdeaManagement.Shared.Entities;

namespace IdeaManagement.API.Services;

public class CategoryService(ICategoryRepository categoryRepository) : ICategoryService
{
    public async Task CreateCategory(string title) =>
        await categoryRepository.CreateCategory(title);

    public List<Category> GetAllCategories() => 
        categoryRepository.GetAllCategories();

    public Category FindCategoryById(string categoryId) => 
        categoryRepository.FindCategoryById(categoryId);

    public async Task DeleteCategoryById(string categoryId) => 
        await categoryRepository.DeleteCategoryById(categoryId);

    public async Task UpdateCategoryTitleById(string categoryId, string newTitle) =>
        await categoryRepository.UpdateCategoryTitleById(categoryId, newTitle);
}