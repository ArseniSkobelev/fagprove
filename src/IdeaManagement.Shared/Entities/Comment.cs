namespace IdeaManagement.Shared.Entities;

public class Comment : BaseEntity
{
    public required string Content { get; set; }
    public required string AuthorId { get; set; }
    public required string IdeaId { get; set; }
    public string? RepliesToCommentId { get; set; }
}