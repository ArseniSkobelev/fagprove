using IdeaManagement.API.Extensions;
using IdeaManagement.API.Repositories;
using IdeaManagement.Shared.DTOs;
using IdeaManagement.Shared.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdeaManagement.API.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class CommentsController(ICommentRepository commentRepository) : ControllerBase
{
    [HttpPost("cmd_add_comment")]
    public async Task<IActionResult> AddCommentCommandHandler([FromBody] Commands.AddCommentCommand cmd)
    {
        try
        {
            var userId = HttpContext.GetUserId();

            if (userId == null)
                return Unauthorized();

            await commentRepository.AddComment(cmd.IdeaId, cmd.RepliesToCommentId, userId, cmd.Content);

            return Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Problem();
        }
    }

    [HttpGet("qry_get_idea_comments/{ideaId}")]
    public async Task<IActionResult> GetIdeaCommentsQueryHandler(string ideaId)
    {
        try
        {
            var userId = HttpContext.GetUserId();

            if (userId == null)
                return Unauthorized();

            var comments = await commentRepository.GetComments(ideaId);

            return Ok(comments);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Problem();
        }
    }

    [HttpPost("cmd_delete_comment/{commentId}")]
    public async Task<IActionResult> DeleteCommentCommandHandler(string commentId)
    {
        try
        {
            var userId = HttpContext.GetUserId();

            if (userId == null)
                return Unauthorized();

            await commentRepository.DeleteComment(commentId, userId);

            return Ok();
        }
        catch (DatabaseExceptions.DocumentNotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (UnauthorizedAccessException e)
        {
            return Unauthorized(e.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Problem();
        }
    }
    
    [HttpPost("cmd_update_comment_content/{commentId}")]
    public async Task<IActionResult> UpdateCommentContentCommandHandler(string commentId, [FromBody] Commands.SingleStringUpdateCommand cmd)
    {
        try
        {
            var userId = HttpContext.GetUserId();

            if (userId == null)
                return Unauthorized();

            await commentRepository.UpdateCommentContent(commentId, cmd.NewValue, userId);

            return Ok();
        }
        catch (DatabaseExceptions.DocumentNotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (UnauthorizedAccessException e)
        {
            return Unauthorized(e.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Problem();
        }
    }
}