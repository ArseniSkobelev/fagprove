using IdeaManagement.Shared.DTOs;

namespace IdeaManagement.API.Services;

public interface IAuth0Service
{
    public Task<List<DTOs.ApplicationUser>> GetApplicationUsers();
    public Task<DTOs.ApplicationUser> GetUserById(string userId);
    public Task<List<DTOs.ApplicationRole>> GetAllRoles();
    public Task SetUserRole(string userId, string roleId, string? currRoleId);
    public Task BlockUser(string userId);
    public Task UnblockUser(string userId);
}