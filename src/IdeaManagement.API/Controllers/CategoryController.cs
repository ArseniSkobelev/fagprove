using IdeaManagement.API.Services;
using IdeaManagement.Shared;
using IdeaManagement.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdeaManagement.API.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class CategoryController(ICategoryService categoryService) : ControllerBase
{
    [Authorize(Roles = Roles.Administrator)]
    [HttpPost("cmd_create_category")]
    public async Task<IActionResult> CreateCategoryCommandHandler(Commands.CreateCategoryCommand cmd)
    {
        await categoryService.CreateCategory(cmd.Title);
        return Ok();
    }

    [HttpGet("qry_get_all_categories")]
    public IActionResult GetAllCategoriesQueryHandler()
    {
        var categories = categoryService.GetAllCategories();
        return Ok(categories);
    }

    [Authorize(Roles = Roles.Administrator)]
    [HttpPost("cmd_delete_category/{categoryId}")]
    public async Task<IActionResult> DeleteCategoryByIdCommandHandler(string categoryId)
    {
        await categoryService.DeleteCategoryById(categoryId);
        return Ok();
    }
    
    [Authorize(Roles = Roles.Administrator)]
    [HttpPost("cmd_update_category_title/{categoryId}")]
    public async Task<IActionResult> UpdateCategoryTitleByIdCommandHandler(string categoryId, [FromBody] Commands.UpdateCategoryTitleByIdCommand cmd)
    {
        await categoryService.UpdateCategoryTitleById(categoryId, cmd.NewTitle);
        return Ok();
    }
}