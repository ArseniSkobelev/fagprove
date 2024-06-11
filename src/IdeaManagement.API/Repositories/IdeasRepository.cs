using IdeaManagement.Shared;
using IdeaManagement.Shared.DTOs;
using IdeaManagement.Shared.Entities;
using MongoDB.Driver;

namespace IdeaManagement.API.Repositories;

public class IdeasRepository(IMongoDatabase db, IStatusRepository statusRepository) : IIdeasRepository
{
    private readonly IMongoCollection<Idea> _ideaCollection = 
        db.GetCollection<Idea>(Constants.DatabaseCollections.Ideas);
    
    private readonly IMongoCollection<Vote> _voteCollection = 
        db.GetCollection<Vote>(Constants.DatabaseCollections.Votes);

    public List<DTOs.IdeaSlim> GetAllIdeas() =>
        _ideaCollection
            .AsQueryable()
            .Select(x => new DTOs.IdeaSlim(x.Id, x.Title, x.AuthorHandle, x.UpdatedAt, x.CreatedAt, x.Status.Value, x.Upvotes))
            .ToList();

    public Task<Idea> GetIdeaDetails(string ideaId)
    {
        throw new NotImplementedException();
    }

    public async Task CreateIdea(string title, string? description, string authorId,
        string authorHandle, string statusId, string categoryId)
    {
        var status = statusRepository.GetStatusById(statusId);
        
        await _ideaCollection.InsertOneAsync(new Idea()
        {
            Title = title,
            Description = description,
            AuthorId = authorId,
            AuthorHandle = authorHandle,
            Status = new KeyValuePair<string, string>(statusId, status.Title),
            Upvotes = 0,
            CategoryId = categoryId
        });
    }

    public async Task UpvoteIdea(string ideaId, string userId)
    {
        var votes = _voteCollection
            .AsQueryable()
            .ToList();

        if (votes.FirstOrDefault(x => x.UserId == userId && x.IdeaId == ideaId) != null)
        {
            throw new Exception("Unable to upvote an idea multiple times");
        }
        
        var ideaFilter = Builders<Idea>.Filter.Eq(x => x.Id, ideaId);

        await _ideaCollection.UpdateOneAsync(ideaFilter, Builders<Idea>.Update.Inc(x => x.Upvotes, 1));

        await _voteCollection.InsertOneAsync(new Vote()
        {
            UserId = userId,
            IdeaId = ideaId
        });
    }

    public async Task RemoveVote(string ideaId, string userId)
    {
        var ideaFilter = Builders<Idea>.Filter.Eq(x => x.Id, ideaId);

        await _ideaCollection.UpdateOneAsync(ideaFilter, Builders<Idea>.Update.Inc(x => x.Upvotes, -1));

        var voteFilterBuilder = Builders<Vote>.Filter;

        var voteFilter = voteFilterBuilder.Eq(x => x.IdeaId, ideaId) &
                         voteFilterBuilder.Eq(x => x.UserId, userId);
        
        await _voteCollection.DeleteOneAsync(voteFilter);
    }

    public List<string> GetUpvotedUserIdeas(string userId) =>
        _voteCollection
            .AsQueryable()
            .Where(x => x.UserId == userId)
            .Select(x => x.IdeaId)
            .ToList();
}