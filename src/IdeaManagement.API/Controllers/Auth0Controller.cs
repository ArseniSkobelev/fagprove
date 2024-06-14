using IdeaManagement.API.Services;
using IdeaManagement.Shared;
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
}