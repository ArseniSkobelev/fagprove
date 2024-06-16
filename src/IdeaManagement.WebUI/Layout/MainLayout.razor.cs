using IdeaManagement.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.SignalR.Client;
using Radzen;
using HubConnectionBuilder = IdeaManagement.WebUI.Domain.SignalR.HubConnectionBuilder;

namespace IdeaManagement.WebUI.Layout;

public partial class MainLayout
{
    [Inject] 
    public NotificationService NotificationService { get; set; } = default!;
    
    [Inject] 
    public HubConnectionBuilder CustomHubConnectionBuilder { get; set; } = default!;
    
    [Inject] 
    public IConfiguration Configuration { get; set; } = default!;
    
    [Inject] 
    public NavigationManager NavigationManager { get; set; } = default!;

    [Inject] 
    public SignOutSessionStateManager SignOutSessionStateManager { get; set; } = default!;
    
    private bool _sidebarExpanded = true;
    private HubConnection? _ideaHub = null;
    private HubConnection? _auth0Hub = null;

    protected override async Task OnInitializedAsync()
    {
        await InitializeHubConnection();
    }

    private async Task InitializeHubConnection(bool retry = false)
    {
        var restApiUri = Configuration["restapi_uri"];

        if (string.IsNullOrWhiteSpace(restApiUri))
            throw new Exception("Missing REST API uri configuration");

        _ideaHub = await CustomHubConnectionBuilder
            .BuildAuthenticatedHubConnectionAsync($"{restApiUri}{Constants.SignalRHubs.Ideas}");
        
        _auth0Hub = await CustomHubConnectionBuilder
            .BuildAuthenticatedHubConnectionAsync($"{restApiUri}{Constants.SignalRHubs.Auth0}");

        if (_ideaHub == null || _auth0Hub == null)
        {
            NotificationService.Notify(NotificationSeverity.Error,
                "Unable to enable notifications. Please click this notification to retry connection",
                closeOnClick: true,
                click: async (nm) => { await InitializeHubConnection(true); }, duration: 10000);
            return;
        }

        _ideaHub.On<string, string, string, bool>(SignalR.Methods.NewIdeaAdded, HandleNewIdeaAdded);
        _ideaHub.On<string, string>(SignalR.Methods.NewCommentAdded, HandleNewCommentAdded);
        _ideaHub.On<string, string, string>(SignalR.Methods.IdeaStatusChanged, HandleIdeaStatusChanged);
        _ideaHub.On(SignalR.Methods.IdeasUpdated, HandleIdeasUpdated);
        _auth0Hub.On(SignalR.Methods.UserRoleUpdated, HandleUserRoleUpdated);
        _auth0Hub.On(SignalR.Methods.UserBlockStatusUpdated, HandleUserBlockStatusUpdated);

        await _ideaHub.StartAsync();
        await _auth0Hub.StartAsync();

        if (retry)
        {
            NotificationService.Notify(NotificationSeverity.Success, "Notifications enabled successfully");
        }
    }

    private async Task HandleUserBlockStatusUpdated()
    {
        await SignOutSessionStateManager.SetSignOutState();
        NavigationManager.NavigateTo("authentication/logout");
    }

    private async Task HandleUserRoleUpdated()
    {
        await SignOutSessionStateManager.SetSignOutState();
        NavigationManager.NavigateTo("authentication/logout");
    }

    private async Task HandleIdeasUpdated()
    {
        NotificationService.Notify(NotificationSeverity.Warning, "Some ideas have recently been updated. Please mind that some information may be outdated", duration: 10000);
    }

    private async Task HandleNewIdeaAdded(string authorHandle, string ideaTitle, string categoryTitle, bool targetedToCategoryOwner)
    {
        NotificationService.Notify(NotificationSeverity.Info, "New idea added",
            !targetedToCategoryOwner
                ? $"{authorHandle} added a new idea \"{ideaTitle}\""
                : $"{authorHandle} added a new idea \"{ideaTitle}\" in your category \"{categoryTitle}\"",
            duration: 10000);
    }
    
    private async Task HandleNewCommentAdded(string ideaId, string ideaTitle)
    {
        NotificationService.Notify(NotificationSeverity.Info, "New comment added", $"New comment has been added to idea \"{ideaTitle}\". Click the notification to navigate to the idea", click: (ns) => HandleNewCommentAddedClick(ideaId), duration: 10000);
    }

    private void HandleNewCommentAddedClick(string ideaId)
    {
        // double navigation because state management is struggling to ensure that everything is loaded in time
        NavigationManager.NavigateTo($"/");
        NavigationManager.NavigateTo($"/idea/{ideaId}");
    }
    
    private async Task HandleIdeaStatusChanged(string ideaId, string statusTitle, string ideaTitle)
    {
        NotificationService.Notify(NotificationSeverity.Info, "Idea status changed", $"Idea status changed for \"{ideaTitle}\". New status \"{statusTitle}\"", duration: 10000);
    }
}