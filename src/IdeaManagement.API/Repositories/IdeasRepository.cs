using IdeaManagement.Shared;
using IdeaManagement.Shared.DTOs;
using IdeaManagement.Shared.Entities;
using IdeaManagement.Shared.Exceptions;
using MongoDB.Driver;

namespace IdeaManagement.API.Repositories;

public class IdeasRepository(IMongoDatabase db, IStatusRepository statusRepository, ICategoryRepository categoryRepository) : IIdeasRepository
{
    private readonly IMongoCollection<Idea> _ideaCollection = 
        db.GetCollection<Idea>(Constants.DatabaseCollections.Ideas);
    
    private readonly IMongoCollection<Vote> _voteCollection = 
        db.GetCollection<Vote>(Constants.DatabaseCollections.Votes);

    public List<DTOs.IdeaSlim> GetAllIdeas()
    {
        List<DTOs.IdeaSlim> ideas = new List<DTOs.IdeaSlim>();
        
        foreach (var idea in _ideaCollection.AsQueryable().ToList())
        {
            var categoryTitle = "Unknown";
            var statusTitle = "Unknown";

            try
            {
                var category = categoryRepository.FindCategoryById(idea.CategoryId);
                categoryTitle = category.Title;
            }
            catch (DatabaseExceptions.DocumentNotFoundException e)
            {
                // omitted
            }

            try
            {
                var status = statusRepository.GetStatusById(idea.StatusId);
                statusTitle = status.Title;
            }
            catch (DatabaseExceptions.DocumentNotFoundException e)
            {
                // omitted
            }
            
            ideas.Add(new DTOs.IdeaSlim(idea.Id, idea.Title, idea.Author.Value, idea.UpdatedAt, idea.CreatedAt, statusTitle, idea.Upvotes, categoryTitle));
        }

        return ideas;
    }

    public DTOs.IdeaFull GetIdeaDetails(string ideaId)
    {
        var idea = _ideaCollection
            .AsQueryable()
            .FirstOrDefault(x => x.Id == ideaId) ?? throw new DatabaseExceptions.DocumentNotFoundException("Idea not found");

        var categoryTitle = "Unknown";
        var statusTitle = "Unknown";

        try
        {
            var category = categoryRepository.FindCategoryById(idea.CategoryId);
            categoryTitle = category.Title;
        }
        catch (DatabaseExceptions.DocumentNotFoundException e)
        {
            // omitted
        }

        try
        {
            var status = statusRepository.GetStatusById(idea.StatusId);
            statusTitle = status.Title;
        }
        catch (DatabaseExceptions.DocumentNotFoundException e)
        {
            // omitted
        }

        return new DTOs.IdeaFull(idea.Id, idea.Title, idea.Description, new (idea.AuthorId, idea.AuthorHandle), idea.LatestEditor,
            idea.UpdatedAt, idea.CreatedAt, statusTitle, idea.Upvotes, categoryTitle);
    }

    public async Task CreateIdea(string title, string? description, string authorId,
        string authorHandle, string statusId, string categoryId)
    {
        
        await _ideaCollection.InsertOneAsync(new Idea()
        {
            Title = title,
            Description = description,
            AuthorId = authorId,
            AuthorHandle = authorHandle,
            StatusId = statusId,
            Upvotes = 0,
            CategoryId = categoryId,
            Author = new KeyValuePair<string, string>(authorId, authorHandle)
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

    public Task DeleteIdea(string ideaId) =>
        _ideaCollection
            .DeleteOneAsync(Builders<Idea>.Filter.Eq(x => x.Id, ideaId));

    public async Task UpdateIdeaTitle(string ideaId, string newTitle)
    {
        var filter = Builders<Idea>.Filter.Eq(x => x.Id, ideaId);

        var update = Builders<Idea>.Update.Set(x => x.Title, newTitle);

        await _ideaCollection.UpdateOneAsync(filter, update);
    }

    public async Task UpdateIdeaDescription(string ideaId, string newDescription)
    {
        var filter = Builders<Idea>.Filter.Eq(x => x.Id, ideaId);

        var update = Builders<Idea>.Update.Set(x => x.Description, newDescription);

        await _ideaCollection.UpdateOneAsync(filter, update);
    }

    public async Task UpdateIdeaStatus(string ideaId, string newStatusId)
    {
        var filter = Builders<Idea>.Filter.Eq(x => x.Id, ideaId);

        var update = Builders<Idea>.Update.Set(x => x.StatusId, newStatusId);

        await _ideaCollection.UpdateOneAsync(filter, update);
    }

    public async Task UpdateIdeaCategory(string ideaId, string newCategoryId)
    {
        var filter = Builders<Idea>.Filter.Eq(x => x.Id, ideaId);

        var update = Builders<Idea>.Update.Set(x => x.CategoryId, newCategoryId);

        await _ideaCollection.UpdateOneAsync(filter, update);
    }
}