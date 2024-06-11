using System.Security.Claims;

namespace IdeaManagement.API.Extensions;

public static class HttpContextExtensions
{
    public static string? GetUserId(this HttpContext context) =>
        context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
}
