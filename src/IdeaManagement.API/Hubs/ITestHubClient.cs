namespace IdeaManagement.API.Hubs;

public interface ITestHubClient
{
    public Task SendMessage(string message);
}