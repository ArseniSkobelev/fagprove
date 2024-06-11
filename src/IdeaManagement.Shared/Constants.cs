namespace IdeaManagement.Shared;

public static class Constants
{
    public static class DatabaseCollections
    {
        public const string Ideas = "ideas";
        public const string Status = "status";
        public const string Categories = "categories";
        public const string Votes = "votes";
    }

    public static class SignalRHubs
    {
        public const string Base = "/hubs";
    }

    public static class DefaultStatuses
    {
        public const string NewIdea = "New idea";
    }
}