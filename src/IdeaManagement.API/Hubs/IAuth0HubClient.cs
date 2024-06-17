namespace IdeaManagement.API.Hubs;

public interface IAuth0HubClient
{
    public Task UserRoleUpdated(string userId);
    public Task UserBlockStatusUpdated(string userId);
}