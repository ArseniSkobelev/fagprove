using IdeaManagement.API.Repositories;
using IdeaManagement.Shared.DTOs;
using IdeaManagement.Shared.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver.Linq;

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

    public async Task UpdateCategoryOwner(string categoryId, string ownerId) =>
        await categoryRepository.UpdateCategoryOwner(categoryId, ownerId);

    public List<Category> GetUserOwnedCategories(string userId)
    {
        var categories = categoryRepository.GetAllCategories();

        var userOwnedCategories = categories.Where(x => x.OwnerId == userId).ToList();

        return userOwnedCategories;
    }

    public async Task RemoveCategoryOwner(string userId)
    {
        var categories = categoryRepository.GetAllCategories();

        var userOwnedCategories = categories.Where(x => x.OwnerId == userId);

        foreach (var category in userOwnedCategories)
        {
            await categoryRepository.UpdateCategoryOwner(category.Id, null);
        }
    }
}