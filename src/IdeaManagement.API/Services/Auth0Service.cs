using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using Auth0.ManagementApi.Paging;
using IdeaManagement.Shared.DTOs;
using IdeaManagement.Shared.Exceptions;
using Microsoft.IdentityModel.Protocols.Configuration;

namespace IdeaManagement.API.Services;

public class Auth0Service(IConfiguration config) : IAuth0Service
{
    public async Task<List<DTOs.ApplicationUser>> GetApplicationUsers()
    {
        var domain = config["auth0_authority"];
        var token = config["auth0_mgmt_api_token"];

        if (string.IsNullOrWhiteSpace(domain) || string.IsNullOrWhiteSpace(token))
            throw new InvalidConfigurationException("Auth0 configuration missing");
        
        var client = new ManagementApiClient(token, new Uri($"{domain}/api/v2"));

        var users = await client.Users.GetAllAsync(new GetUsersRequest(), new PaginationInfo(0, 100));

        if (users == null)
            return new List<DTOs.ApplicationUser>();

        List<DTOs.ApplicationUser> _users = new List<DTOs.ApplicationUser>();
        
        foreach (var user in users.ToList())
        {
            var userRoles = await client.Users.GetRolesAsync(user.UserId);

            if (userRoles == null)
                continue;

            var roleList = new List<string>();
            
            userRoles.ToList().ForEach(x => roleList.Add(x.Name));
            
            _users.Add(new DTOs.ApplicationUser(user.UserId, user.FullName, user.Email, string.Join(",", roleList), user.Blocked ?? false));
        }

        return _users;
    }

    public async Task<DTOs.ApplicationUser> GetUserById(string userId)
    {
        var domain = config["auth0_authority"];
        var token = config["auth0_mgmt_api_token"];
        
        if (string.IsNullOrWhiteSpace(domain) || string.IsNullOrWhiteSpace(token))
            throw new InvalidConfigurationException("Auth0 configuration missing");
        
        var client = new ManagementApiClient(token, new Uri($"{domain}/api/v2"));
        
        var user = await client.Users.GetAsync(userId);
        
        return new DTOs.ApplicationUser(user.UserId, user.FullName, user.Email, "Unknown", user.Blocked ?? false) ?? throw new DatabaseExceptions.DocumentNotFoundException("User not found");
    }

    public async Task<List<DTOs.ApplicationRole>> GetAllRoles()
    {
        var domain = config["auth0_authority"];
        var token = config["auth0_mgmt_api_token"];

        if (string.IsNullOrWhiteSpace(domain) || string.IsNullOrWhiteSpace(token))
            throw new InvalidConfigurationException("Auth0 configuration missing");
        
        var client = new ManagementApiClient(token, new Uri($"{domain}/api/v2"));

        var roles = await client.Roles.GetAllAsync(new GetRolesRequest());

        if (roles == null)
            return new List<DTOs.ApplicationRole>();

        return roles.Select(x => new DTOs.ApplicationRole(x.Name, x.Id)).ToList();
    }

    public async Task SetUserRole(string userId, string roleId, string? currRoleId)
    {
        var domain = config["auth0_authority"];
        var token = config["auth0_mgmt_api_token"];

        if (string.IsNullOrWhiteSpace(domain) || string.IsNullOrWhiteSpace(token))
            throw new InvalidConfigurationException("Auth0 configuration missing");
        
        var client = new ManagementApiClient(token, new Uri($"{domain}/api/v2"));

        // remove curr role
        if (!string.IsNullOrWhiteSpace(currRoleId))
        {
            var roleRemovalRequest = new AssignRolesRequest();

            roleRemovalRequest.Roles = new[] { currRoleId };
            
            await client.Users.RemoveRolesAsync(userId, roleRemovalRequest);
        }

        var rolesAssignmentRequest = new AssignRolesRequest();

        rolesAssignmentRequest.Roles = new[] { roleId };
        
        // assign required role
        await client.Users.AssignRolesAsync(userId, rolesAssignmentRequest);
    }

    public async Task BlockUser(string userId)
    {
        var domain = config["auth0_authority"];
        var token = config["auth0_mgmt_api_token"];

        if (string.IsNullOrWhiteSpace(domain) || string.IsNullOrWhiteSpace(token))
            throw new InvalidConfigurationException("Auth0 configuration missing");
        
        var client = new ManagementApiClient(token, new Uri($"{domain}/api/v2"));

        var updateRequest = new UserUpdateRequest();

        updateRequest.Blocked = true;

        await client.Users.UpdateAsync(userId, updateRequest);
    }

    public async Task UnblockUser(string userId)
    {
        var domain = config["auth0_authority"];
        var token = config["auth0_mgmt_api_token"];

        if (string.IsNullOrWhiteSpace(domain) || string.IsNullOrWhiteSpace(token))
            throw new InvalidConfigurationException("Auth0 configuration missing");
        
        var client = new ManagementApiClient(token, new Uri($"{domain}/api/v2"));

        var updateRequest = new UserUpdateRequest();

        updateRequest.Blocked = false;

        await client.Users.UpdateAsync(userId, updateRequest);
    }
}