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
    
    [HttpPost("cmd_delete_idea/{ideaId}")]
    public async Task<IActionResult> DeleteIdea(string ideaId)
    {
        try
        {
            var userId = HttpContext.GetUserId();

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            await _ideasService.DeleteIdea(ideaId, userId);
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

    [HttpGet("qry_get_article_details/{ideaId}")]
    public IActionResult GetArticleDetailsQueryHandler(string ideaId)
    {
        try
        {
            var idea = _ideasService.GetIdeaDetails(ideaId);
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

            await _ideasService.UpdateIdeaTitle(ideaId, userId, cmd.NewValue);

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

            await _ideasService.UpdateIdeaDescription(ideaId, userId, cmd.NewValue);

            return Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Problem();
        }
    }
    
    [HttpPost("cmd_update_status/{ideaId}")]
    public async Task<IActionResult> UpdateIdeaStatusCommandHandler(string ideaId, [FromBody] Commands.UpdateIdeaStatusCommand cmd)
    {
        try
        {
            var userId = HttpContext.GetUserId();

            if (userId == null)
                return Unauthorized();

            await _ideasService.UpdateIdeaStatus(ideaId, userId, cmd.StatusId);

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
    
    [HttpPost("cmd_update_category/{ideaId}")]
    public async Task<IActionResult> UpdateIdeaCategoryCommandHandler(string ideaId, [FromBody] Commands.UpdateIdeaCategoryCommand cmd)
    {
        try
        {
            var userId = HttpContext.GetUserId();

            if (userId == null)
                return Unauthorized();

            await _ideasService.UpdateIdeaCategory(ideaId, userId, cmd.CategoryId);

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