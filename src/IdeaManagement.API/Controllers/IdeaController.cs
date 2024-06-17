using System.Security.Claims;
using Auth0.ManagementApi.Models;
using IdeaManagement.API.Extensions;
using IdeaManagement.API.Hubs;
using IdeaManagement.API.Repositories;
using IdeaManagement.API.Services;
using IdeaManagement.Shared;
using IdeaManagement.Shared.DTOs;
using IdeaManagement.Shared.Entities;
using IdeaManagement.Shared.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace IdeaManagement.API.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class IdeaController(IIdeasService ideasService, IIdeasRepository ideasRepository, IHubContext<IdeaHub> ideaHubContext, IAuth0Service auth0Service, ICategoryService categoryService, IStatusService statusService) : ControllerBase
{
    private readonly IdeaHub _ideaHub = new IdeaHub(ideaHubContext, auth0Service, ideasService, categoryService);

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

            var idea = await ideasService
                .CreateIdea(cmd.Title, cmd.Description, userId, authorHandle, cmd.CategoryId);

            await _ideaHub.NotifyNewIdeaAdded(idea.AuthorId, idea.Id, idea.CategoryId);

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
        var ideas = ideasService.GetAllIdeas();

        return Ok(ideas);
    }
    
    [HttpPost("cmd_delete_idea/{ideaId}")]
    public async Task<IActionResult> DeleteIdea(string ideaId)
    {
        try
        {
            var userId = HttpContext.GetUserId();

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            await ideasService.DeleteIdea(ideaId, userId);

            await _ideaHub.NotifyIdeasUpdated();

            return Ok();
        }
        catch (UnauthorizedAccessException e)
        {
            return Unauthorized(e.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Problem(e.Message);
        }
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

    [HttpGet("qry_get_idea_details/{ideaId}")]
    public IActionResult GetIdeaDetailsQueryHandler(string ideaId)
    {
        try
        {
            var idea = ideasService.GetIdeaDetails(ideaId);
            return Ok(idea);
        }
        catch (DatabaseExceptions.DocumentNotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    [HttpPost("cmd_update_title/{ideaId}")]
    public async Task<IActionResult> UpdateIdeaTitleCommandHandler(string ideaId, [FromBody] Commands.SingleStringUpdateCommand cmd)
    {
        try
        {
            var userId = HttpContext.GetUserId();
            
            if (userId == null)
                return Unauthorized();

            await ideasService.UpdateIdeaTitle(ideaId, userId, cmd.NewValue);

            return Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Problem();
        }
    }
    
    [HttpPost("cmd_update_description/{ideaId}")]
    public async Task<IActionResult> UpdateIdeaDescriptionCommandHandler(string ideaId, [FromBody] Commands.SingleStringUpdateCommand cmd)
    {
        try
        {
            var userId = HttpContext.GetUserId();
            
            if (userId == null)
                return Unauthorized();

            await ideasService.UpdateIdeaDescription(ideaId, userId, cmd.NewValue);

            return Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Problem();
        }
    }
    
    [Authorize(Roles = $"{Roles.Administrator},{Roles.CategoryOwner}")]
    [HttpPost("cmd_update_status/{ideaId}")]
    public async Task<IActionResult> UpdateIdeaStatusCommandHandler(string ideaId, [FromBody] Commands.UpdateIdeaStatusCommand cmd)
    {
        try
        {
            var userId = HttpContext.GetUserId();

            if (userId == null)
                return Unauthorized();

            await ideasService.UpdateIdeaStatus(ideaId, userId, cmd.StatusId);

            var ideaDetails = ideasService.GetIdeaDetails(ideaId);
            var statusDetails = statusService.FindStatusById(cmd.StatusId);

            await _ideaHub.NotifyIdeaStatusChanged(ideaId, statusDetails.Title, ideaDetails.Title);

            return Ok();
        }
        catch (DatabaseExceptions.DocumentNotFoundException e)
        {
            Console.WriteLine(e);
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Problem();
        }
    }

    [Authorize(Roles = $"{Roles.Administrator},{Roles.CategoryOwner}")]
    [HttpPost("cmd_update_category/{ideaId}")]
    public async Task<IActionResult> UpdateIdeaCategoryCommandHandler(string ideaId, [FromBody] Commands.UpdateIdeaCategoryCommand cmd)
    {
        try
        {
            var userId = HttpContext.GetUserId();

            if (userId == null)
                return Unauthorized();

            await ideasService.UpdateIdeaCategory(ideaId, userId, cmd.CategoryId);

            return Ok();
        }
        catch (DatabaseExceptions.DocumentNotFoundException e)
        {
            Console.WriteLine(e);
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Problem();
        }
    }
}