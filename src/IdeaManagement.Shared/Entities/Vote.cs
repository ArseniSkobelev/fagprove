namespace IdeaManagement.Shared.Entities;

public class Vote : BaseEntity
{
    public required string UserId { get; set; }
    public required string IdeaId { get; set; }
}