using IdeaManagement.API.Services;
using IdeaManagement.Shared;
using IdeaManagement.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdeaManagement.API.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class StatusController(IStatusService _statusService) : ControllerBase
{
    [Authorize(Roles = Roles.Administrator)]
    [HttpPost("cmd_create_status")]
    public async Task<IActionResult> CreateStatusCommandHandler(Commands.CreateStatusCommand cmd)
    {
        await _statusService.CreateStatus(cmd.Title);
        return Ok();
    }

    [HttpGet("qry_get_all_statuses")]
    public IActionResult GetAllStatusesQueryHandler()
    {
        var statuses = _statusService.GetAllStatuses();
        return Ok(statuses);
    }

    [Authorize(Roles = Roles.Administrator)]
    [HttpPost("cmd_delete_status/{statusId}")]
    public async Task<IActionResult> DeleteStatusByIdCommandHandler(string statusId)
    {
        await _statusService.DeleteStatusById(statusId);
        return Ok();
    }
    
    [Authorize(Roles = Roles.Administrator)]
    [HttpPost("cmd_update_status_title/{statusId}")]
    public async Task<IActionResult> UpdateStatusTitleByIdCommandHandler(string statusId, [FromBody] Commands.UpdateStatusTitleByIdCommand cmd)
    {
        await _statusService.UpdateStatusTitleById(statusId, cmd.NewTitle);
        return Ok();
    }
}