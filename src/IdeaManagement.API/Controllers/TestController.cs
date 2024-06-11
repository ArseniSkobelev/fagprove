using IdeaManagement.API.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace IdeaManagement.API.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    private readonly TestHub _testHub;

    public TestController(IHubContext<TestHub> testHubContext)
    {
        _testHub = new TestHub(testHubContext);
    }
    
    [HttpGet]
    public IActionResult Test()
    {
        return Ok("Hello, world!");
    }

    [HttpGet("hub")]
    public async Task<IActionResult> TestHub()
    {
        await _testHub.SendMessage("test");
        return Ok();
    }
}