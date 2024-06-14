using IdeaManagement.Shared.DTOs;

namespace IdeaManagement.API.Repositories;

public interface IControllerRepository
{
    public Task AddComment(string ideaId, string? repliesToCommentId, string authorId, string content);
    public Task DeleteComment(string commentId);
    public Task UpdateCommentContent(string commentId, string newContent);
    public Task<List<DTOs.CommentDetails>> GetComments(string ideaId);
}