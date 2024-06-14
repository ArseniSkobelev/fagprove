using IdeaManagement.Shared.DTOs;

namespace IdeaManagement.API.Services;

public interface IAuth0Service
{
    public Task<List<DTOs.ApplicationUser>> GetApplicationUsers();
    public Task<DTOs.ApplicationUser> GetUserById(string userId);
}