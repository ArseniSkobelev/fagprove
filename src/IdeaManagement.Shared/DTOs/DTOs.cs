namespace IdeaManagement.Shared.DTOs;

public static class DTOs
{
    public record IdeaSlim(
        string Id,
        string Title,
        string AuthorHandle,
        DateTime UpdatedAt,
        DateTime CreatedAt,
        string Status,
        int Upvotes,
        string Category);

    public record IdeaFull(
        string Id,
        string Title,
        string? Description,
        KeyValuePair<string, string> Author,
        KeyValuePair<string, string>? LatestEditor,
        DateTime UpdatedAt,
        DateTime CreatedAt,
        string Status,
        int Upvotes,
        KeyValuePair<string, string> Category);

    public record ApplicationUser(
        string UserId,
        string Name,
        string Email,
        string Role);

    public record ApplicationRole(string Title, string Id);

    public record CommentDetails(
        string Id,
        string Content,
        string AuthorHandle,
        string? RepliesToCommentId,
        DateTime CreatedAt,
        string AuthorId);
}