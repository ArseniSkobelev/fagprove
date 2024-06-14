using IdeaManagement.Shared.DTOs;

namespace IdeaManagement.API.Repositories;

public interface ICommentRepository
{
    public Task AddComment(string ideaId, string? repliesToCommentId, string authorId, string content);
    public Task DeleteComment(string commentId, string authorId);
    public Task UpdateCommentContent(string commentId, string newContent, string userId);
    public Task<List<DTOs.CommentDetails>> GetComments(string ideaId);
}