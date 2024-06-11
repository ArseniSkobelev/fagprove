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
        int Upvotes);
}