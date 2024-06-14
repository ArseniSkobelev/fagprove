using IdeaManagement.API.Hubs;
using IdeaManagement.API.Services;
using IdeaManagement.Shared;
using IdeaManagement.Shared.DTOs;
using IdeaManagement.Shared.Entities;
using IdeaManagement.Shared.Exceptions;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace IdeaManagement.API.Repositories;

public class CommentRepository(IMongoDatabase db, IAuth0Service auth0Service, IHubContext<IdeaHub> ideaHub, IIdeasRepository ideasRepository) : ICommentRepository
{
    private readonly IMongoCollection<Comment> _commentCollection = 
        db.GetCollection<Comment>(Constants.DatabaseCollections.Comments);

    public async Task AddComment(string ideaId, string? repliesToCommentId, string authorId, string content)
    {
        var idea = ideasRepository.GetIdeaDetails(ideaId);
        
        await _commentCollection.InsertOneAsync(new Comment()
        {
            Content = content,
            AuthorId = authorId,
            IdeaId = ideaId,
            RepliesToCommentId = repliesToCommentId
        });
        
        await ideaHub.Clients.All.SendAsync(SignalR.Methods.NewCommentAdded, ideaId, idea.Title);
    }

    public async Task DeleteComment(string commentId, string authorId)
    {
        var comment = _commentCollection.AsQueryable().FirstOrDefault(x => x.Id == commentId);

        if (comment == null)
            throw new DatabaseExceptions.DocumentNotFoundException("Comment not found");

        if (comment.AuthorId != authorId)
            throw new UnauthorizedAccessException();
            
        await _commentCollection.DeleteOneAsync(Builders<Comment>.Filter.Eq(x => x.Id, commentId));
    }

    public async Task UpdateCommentContent(string commentId, string newContent, string userId)
    {
        var comment = _commentCollection.AsQueryable().FirstOrDefault(x => x.Id == commentId);

        if (comment == null)
            throw new DatabaseExceptions.DocumentNotFoundException("Comment not found");

        if (comment.AuthorId != userId)
            throw new UnauthorizedAccessException();
        
        var filter = Builders<Comment>.Filter.Eq(x => x.Id, commentId);
        var update = Builders<Comment>.Update.Set(x => x.Content, newContent);

        await _commentCollection.UpdateOneAsync(filter, update);
    }

    public async Task<List<DTOs.CommentDetails>> GetComments(string ideaId)
    {
        var ideaComments = _commentCollection
            .AsQueryable()
            .Where(x => x.IdeaId == ideaId)
            .ToList();

        List<DTOs.CommentDetails> comments = new List<DTOs.CommentDetails>();
        
        foreach (var ideaComment in ideaComments)
        {
            var author = await auth0Service.GetUserById(ideaComment.AuthorId);
            
            comments.Add(new DTOs.CommentDetails(ideaComment.Id, ideaComment.Content, author.Name, ideaComment.RepliesToCommentId, ideaComment.CreatedAt, author.UserId));
        }

        return comments;
    }
}