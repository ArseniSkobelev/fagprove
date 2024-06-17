using IdeaManagement.Shared;
using IdeaManagement.Shared.Entities;
using IdeaManagement.Shared.Exceptions;
using MongoDB.Driver;

namespace IdeaManagement.API.Repositories;

public class CategoryRepository(IMongoDatabase db) : ICategoryRepository
{
    private readonly IMongoCollection<Category> _categoryCollection = 
        db.GetCollection<Category>(Constants.DatabaseCollections.Categories);

    public async Task CreateCategory(string title) =>
        await _categoryCollection.InsertOneAsync(new Category()
        {
            Title = title
        });

    public List<Category> GetAllCategories() =>
        _categoryCollection
            .AsQueryable()
            .ToList();

    public Category FindCategoryById(string categoryId) =>
        _categoryCollection
            .AsQueryable()
            .FirstOrDefault(x => x.Id == categoryId) ?? throw new DatabaseExceptions.DocumentNotFoundException("Category not found");

    public async Task DeleteCategoryById(string categoryId) =>
        await _categoryCollection.DeleteOneAsync(Builders<Category>.Filter.Eq(x => x.Id, categoryId));

    public async Task UpdateCategoryTitleById(string categoryId, string newTitle) =>
        await _categoryCollection
            .UpdateOneAsync(Builders<Category>.Filter.Eq(x => x.Id, categoryId), Builders<Category>.Update.Set(x => x.Title, newTitle));

    public async Task UpdateCategoryOwner(string categoryId, string? ownerId)
    {
        var filter = Builders<Category>.Filter.Eq(x => x.Id, categoryId);
        var update = Builders<Category>.Update.Set(x => x.OwnerId, ownerId);

        await _categoryCollection.UpdateOneAsync(filter, update);
    }
}