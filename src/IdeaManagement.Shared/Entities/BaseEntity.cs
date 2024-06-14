using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace IdeaManagement.Shared.Entities;

public class BaseEntity
{
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonId]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    [BsonRepresentation(BsonType.DateTime)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    [BsonRepresentation(BsonType.DateTime)]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}