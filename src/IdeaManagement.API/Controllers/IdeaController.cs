using System.Security.Claims;
using IdeaManagement.API.Extensions;
using IdeaManagement.API.Repositories;
using IdeaManagement.API.Services;
using IdeaManagement.Shared;
using IdeaManagement.Shared.DTOs;
using IdeaManagement.Shared.Entities;
using IdeaManagement.Shared.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdeaManagement.API.Controllers;

[ApiController]
[Route("[controller]")]
public class IdeaController(IIdeasService _ideasService, IIdeasRepository ideasRepository) : ControllerBase
{
    [Authorize(Roles = $"{Roles.IdeaContributor},{Roles.Administrator},{Roles.CategoryOwner}")]
    [HttpPost("cmd_create_idea")]
    public async Task<IActionResult> CreateIdeaCommandHandler([FromBody] Commands.CreateIdeaCommand cmd)
    {
        try
        {
            var userId = HttpContext.GetUserId();

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            // google auth users may or may not have name set in access_token
            // native users won't have name set
            var email = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
            var fName = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName);
            var sName = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Surname);

            var authorHandle = fName != null && sName != null
                ? $"{fName.Value} {sName.Value}"
                : email?.Value ?? "Unknown";

            await _ideasService
                .CreateIdea(cmd.Title, cmd.Description, userId, authorHandle, cmd.CategoryId);

            return Ok();
        }
        catch (DatabaseExceptions.DocumentNotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);

            return Problem(e.Message);
        }
    }

    [HttpGet("qry_get_all_ideas")]
    public IActionResult GetAllIdeas()
    {
        var ideas = _ideasService.GetAllIdeas();

        return Ok(ideas);
    }
    
    [HttpPost("cmd_upvote_idea/{ideaId}")]
    public async Task<IActionResult> UpvoteIdeaCommandHandler(string ideaId)
    {
        try
        {
            var userId = HttpContext.GetUserId();
            
            if (userId == null)
                return Unauthorized();
            
            await ideasRepository.UpvoteIdea(ideaId, userId);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Problem(e.Message);
        }

        return Ok();
    }
    
    [HttpPost("cmd_remove_vote/{ideaId}")]
    public async Task<IActionResult> RemoveVoteCommandHandler(string ideaId)
    {
        try
        {
            var userId = HttpContext.GetUserId();
            
            if (userId == null)
                return Unauthorized();
            
            await ideasRepository.RemoveVote(ideaId, userId);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Problem(e.Message);
        }

        return Ok();
    }
    
    [HttpGet("qry_get_user_upvotes")]
    public async Task<IActionResult> GetUserUpvotesQueryHandler()
    {
        try
        {
            var userId = HttpContext.GetUserId();
            
            if (userId == null)
                return Unauthorized();
            
            var upvotes = ideasRepository.GetUpvotedUserIdeas(userId);

            return Ok(upvotes);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Problem(e.Message);
        }
    }
}