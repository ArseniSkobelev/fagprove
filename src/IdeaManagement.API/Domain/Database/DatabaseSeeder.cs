using IdeaManagement.Shared;
using IdeaManagement.Shared.Entities;
using MongoDB.Driver;

namespace IdeaManagement.API.Domain.Database;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IMongoDatabase db)
    {
        Console.WriteLine("Seeding database..");
        
        // seed statuses
        var statusCollection = db.GetCollection<Status>(Constants.DatabaseCollections.Status);

        var predefinedStatuses = new List<Status>()
        {
            new Status()
            {
                Title = "New idea"
            },
            new Status()
            {
                Title = "For evaluation"
            },
            new Status()
            {
                Title = "In progress"
            },
            new Status()
            {
                Title = "Waiting"
            },
            new Status()
            {
                Title = "Done"
            },
        };

        foreach (var status in predefinedStatuses)
        {
            var filter = Builders<Status>.Filter.Eq(x => x.Title, status.Title);

            var existingStatus = await statusCollection.Find(filter).FirstOrDefaultAsync();

            if (existingStatus == null)
            {
                await statusCollection.InsertOneAsync(status);
            }
        }
        
        Console.WriteLine("Finished seeding database..");
    }
}