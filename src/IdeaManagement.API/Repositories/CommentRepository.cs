using IdeaManagement.Shared.DTOs;

namespace IdeaManagement.API.Repositories;

public class CommentRepository : ICommentRepository
{
    public Task AddComment(string ideaId, string? repliesToCommentId, string authorId, string content)
    {
        throw new NotImplementedException();
    }

    public Task DeleteComment(string commentId)
    {
        throw new NotImplementedException();
    }

    public Task UpdateCommentContent(string commentId, string newContent)
    {
        throw new NotImplementedException();
    }

    public Task<List<DTOs.CommentDetails>> GetComments(string ideaId)
    {
        throw new NotImplementedException();
    }
}