using IdeaManagement.API.Services;
using IdeaManagement.Shared;
using IdeaManagement.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdeaManagement.API.Controllers;

[Authorize(Roles = Roles.Administrator)]
[ApiController]
[Route("[controller]")]
public class Auth0Controller(IAuth0Service auth0Service) : ControllerBase
{
    [HttpGet("qry_get_application_users")]
    public async Task<IActionResult> GetApplicationUsersQueryHandler()
    {
        try
        {
            var users = await auth0Service.GetApplicationUsers();

            return Ok(users);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Problem();
        }
    }

    [HttpGet("qry_get_all_roles")]
    public async Task<IActionResult> GetAllRolesQueryHandler()
    {
        try
        {
            var roles = await auth0Service.GetAllRoles();

            return Ok(roles);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Problem();
        }
    }

    [HttpPost("cmd_set_user_role")]
    public async Task<IActionResult> SetUserRoleCommandHandler([FromBody] Commands.SetUserRoleCommand cmd)
    {
        try
        {
            await auth0Service.SetUserRole(cmd.userId, cmd.roleId);

            return Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Problem();
        }
    }
}