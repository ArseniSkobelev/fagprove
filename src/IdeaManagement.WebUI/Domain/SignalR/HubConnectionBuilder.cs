using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.SignalR.Client;

namespace IdeaManagement.WebUI.Domain.SignalR;

public class CustomHubConnectionBuilder(IAccessTokenProvider accessTokenProvider,
    AuthenticationStateProvider authenticationStateProvider)
    : HubConnectionBuilder
{

    /// <summary>
    /// Builds an authenticated hub connection
    /// </summary>
    /// <param name="hubUrl"></param>
    /// <returns></returns>
    public override async Task<HubConnection?> BuildAuthenticatedHubConnectionAsync(string hubUrl)
    {
        var authState = await authenticationStateProvider.GetAuthenticationStateAsync();

        if (authState.User.Identity is null || !authState.User.Identity.IsAuthenticated)
            return null;

        var accessTokenResult = await accessTokenProvider.RequestAccessToken();

        if (!accessTokenResult.TryGetToken(out var accessToken))
            return null;

        return new Microsoft.AspNetCore.SignalR.Client.HubConnectionBuilder().WithUrl(hubUrl, options =>
        {
            options.AccessTokenProvider = async () => await Task.FromResult(accessToken.Value);
        }).Build();
    }
}

public abstract class HubConnectionBuilder
{
    public abstract Task<HubConnection?> BuildAuthenticatedHubConnectionAsync(string hubUrl);
}
