namespace IdeaManagement.Shared.DTOs;

public static class Commands
{
    public record CreateIdeaCommand(string Title, string? Description, string CategoryId);
    public record CreateStatusCommand(string Title);
    public record CreateCategoryCommand(string Title);
    public record UpdateCategoryTitleByIdCommand(string NewTitle);
    public record UpdateStatusTitleByIdCommand(string NewTitle);
}