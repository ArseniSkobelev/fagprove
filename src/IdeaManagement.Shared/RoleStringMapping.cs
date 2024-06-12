namespace IdeaManagement.Shared;

public static class RoleStringMapping
{
    public static string GetFriendlyRoleName(string roleName)
    {
        return roleName switch
        {
            "administrator" => "Administrator",
            "idea_contributor" => "Idea contributor",
            "category_owner" => "Category owner",
            _ => "Unknown"
        };
    }
}