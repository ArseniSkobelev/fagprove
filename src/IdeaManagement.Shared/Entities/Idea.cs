namespace IdeaManagement.Shared.Entities;

public class Idea : BaseEntity
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    
    public required string AuthorId { get; set; }
    public required string AuthorHandle { get; set; }

    public int Upvotes { get; set; } = 0;
    
    public required KeyValuePair<string, string> Author { get; set; }
    public KeyValuePair<string, string>? LatestEditor { get; set; } = null;
    
    public required string StatusId { get; set; }
    public required string CategoryId { get; set; }
}