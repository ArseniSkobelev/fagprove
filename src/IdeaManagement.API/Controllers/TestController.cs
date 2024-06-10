using Microsoft.AspNetCore.Mvc;

namespace IdeaManagement.API.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    [HttpGet]
    public IActionResult Test()
    {
        return Ok("Hello, world!");
    }
}