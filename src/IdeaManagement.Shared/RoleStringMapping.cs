namespace IdeaManagement.Shared;

public static class RoleStringMapping
{
    public static string GetFriendlyRoleName(string roleName)
    {
        return roleName switch
        {
            Roles.Administrator => "Administrator",
            Roles.IdeaContributor => "Idea contributor",
            Roles.CategoryOwner => "Category owner",
            _ => "Unknown"
        };
    }
}