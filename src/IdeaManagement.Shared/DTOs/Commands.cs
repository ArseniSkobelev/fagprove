namespace IdeaManagement.Shared.DTOs;

public static class Commands
{
    public record CreateIdeaCommand(string Title, string? Description, string CategoryId);
    public record CreateStatusCommand(string Title);
    public record CreateCategoryCommand(string Title);
    public record UpdateCategoryTitleByIdCommand(string NewTitle);
    public record UpdateStatusTitleByIdCommand(string NewTitle);
    public record SingleStringUpdateCommand(string NewValue);
    public record UpdateIdeaStatusCommand(string StatusId);
    public record UpdateIdeaCategoryCommand(string CategoryId);
    public record UpdateCategoryOwnerCommand(string OwnerId);
    public record AddCommentCommand(string IdeaId, string Content, string? RepliesToCommentId);
    public record SetUserRoleCommand(string UserId, string RoleId, string? CurrRoleId);
    public record BlockUserCommand(string UserId);
    public record UnblockUserCommand(string UserId);
}