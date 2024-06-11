namespace IdeaManagement.Shared;

public static class Constants
{
    public static class DatabaseCollections
    {
        public const string Ideas = "ideas";
    }

    public static class SignalRHubs
    {
        public const string Base = "/hubs";
        public const string TestHub = "/hubs/testhub";
    }
}