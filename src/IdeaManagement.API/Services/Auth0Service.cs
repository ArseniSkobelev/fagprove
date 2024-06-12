using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using Auth0.ManagementApi.Paging;
using IdeaManagement.Shared.DTOs;
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
        
        users.ToList().ForEach(x => _users.Add(new DTOs.ApplicationUser(x.UserId, x.FullName, x.Email)));

        return _users;
    }
}