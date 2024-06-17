using IdeaManagement.API.Hubs;
using IdeaManagement.API.Services;
using IdeaManagement.Shared;
using IdeaManagement.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace IdeaManagement.API.Controllers;

[Authorize(Roles = Roles.Administrator)]
[ApiController]
[Route("[controller]")]
public class Auth0Controller(IAuth0Service auth0Service, IHubContext<Auth0Hub> auth0HubContext, ICategoryService categoryService) : ControllerBase
{
    private readonly Auth0Hub _auth0Hub = new Auth0Hub(auth0HubContext);
    
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
            await auth0Service.SetUserRole(cmd.UserId, cmd.RoleId, cmd.CurrRoleId);

            await categoryService.RemoveCategoryOwner(cmd.UserId);
            
            await _auth0Hub.UserRoleUpdated(cmd.UserId);

            return Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Problem();
        }
    }
    
    [HttpPost("cmd_block_user")]
    public async Task<IActionResult> BlockUserCommandHandler([FromBody] Commands.BlockUserCommand cmd)
    {
        try
        {
            await auth0Service.BlockUser(cmd.UserId);
            
            await _auth0Hub.UserBlockStatusUpdated(cmd.UserId);

            return Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Problem();
        }
    }
    
    [HttpPost("cmd_unblock_user")]
    public async Task<IActionResult> UnblockUserCommandHandler([FromBody] Commands.UnblockUserCommand cmd)
    {
        try
        {
            await auth0Service.UnblockUser(cmd.UserId);

            return Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Problem();
        }
    }
}