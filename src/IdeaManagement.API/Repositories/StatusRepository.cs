using IdeaManagement.Shared;
using IdeaManagement.Shared.Entities;
using IdeaManagement.Shared.Exceptions;
using MongoDB.Driver;

namespace IdeaManagement.API.Repositories;

public class StatusRepository(IMongoDatabase db) : IStatusRepository
{
    private readonly IMongoCollection<Status> _statusCollection = 
        db.GetCollection<Status>(Constants.DatabaseCollections.Status);
    
    public async Task CreateStatus(string title) =>
        await _statusCollection.InsertOneAsync(new Status()
        {
            Title = title,
        });

    public Status GetStatusByTitle(string title) =>
        _statusCollection
            .AsQueryable()
            .FirstOrDefault(x => x.Title == title) ?? throw new DatabaseExceptions.DocumentNotFoundException("Status not found");

    public Status GetStatusById(string statusId) =>
        _statusCollection
            .AsQueryable()
            .FirstOrDefault(x => x.Id == statusId) ?? throw new DatabaseExceptions.DocumentNotFoundException("Status not found");

    public List<Status> GetAllStatuses() =>
        _statusCollection
            .AsQueryable()
            .ToList();

    public async Task DeleteStatusById(string statusId) =>
        await _statusCollection.DeleteOneAsync(Builders<Status>.Filter.Eq(x => x.Id, statusId));

    public async Task UpdateStatusTitleById(string statusId, string newTitle) =>
        await _statusCollection
            .UpdateOneAsync(Builders<Status>.Filter.Eq(x => x.Id, statusId), Builders<Status>.Update.Set(x => x.Title, newTitle));
}