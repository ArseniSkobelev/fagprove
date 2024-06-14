namespace IdeaManagement.Shared.Entities;

public class Category : BaseEntity
{
    public required string Title { get; set; }
    public string? OwnerId { get; set; }
}