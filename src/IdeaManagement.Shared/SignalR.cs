namespace IdeaManagement.Shared;

public static class SignalR
{
    public static class Methods
    {
        public const string NewIdeaAdded = "NewIdeaAdded";
        public const string NewCommentAdded = "NewCommentAdded";
        public const string IdeaStatusChanged = "IdeaStatusChanged";
        public const string IdeasUpdated = "IdeasUpdated";
        public const string UserRoleUpdated = "UserRoleUpdated";
        public const string UserBlockStatusUpdated = "UserBlockStatusUpdated";
    }
}