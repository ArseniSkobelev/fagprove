using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace IdeaManagement.WebUI.Domain.Authentication;

public class CustomAuthorizationMessageHandler : AuthorizationMessageHandler
{
    public CustomAuthorizationMessageHandler(IAccessTokenProvider provider,
        NavigationManager navigationManager, IConfiguration config)
        : base(provider, navigationManager)
    {
        ConfigureHandler(
            authorizedUrls: new[] { config["restapi_uri"] ?? throw new Exception("Missing REST API uri configuration") });
    
    }
}  
