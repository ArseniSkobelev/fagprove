using IdeaManagement.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
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
    
    private bool _sidebarExpanded = true;
    private HubConnection? _ideaHub = null;

    protected override async Task OnInitializedAsync()
    {
        await InitializeSignalRConnection();
    }

    private async Task InitializeSignalRConnection()
    {
        var restApiUri = Configuration["restapi_uri"];

        if (string.IsNullOrWhiteSpace(restApiUri))
            throw new Exception("Missing REST API uri configuration");

        _ideaHub = await CustomHubConnectionBuilder
            .BuildAuthenticatedHubConnectionAsync($"{restApiUri}{Constants.SignalRHubs.Ideas}");

        if (_ideaHub == null)
        {
            NotificationService.Notify(NotificationSeverity.Error, "Unable to enable notifications. Please click this notification to retry connection", click: async (ns) => await InitializeSignalRConnection(), duration: 10000);
            return;
        }

        _ideaHub.On<string, string>(SignalR.Methods.NewIdeaAdded, HandleNewIdeaAdded);
        _ideaHub.On<string, string>(SignalR.Methods.NewCommentAdded, HandleNewCommentAdded);
        _ideaHub.On<string, string, string>(SignalR.Methods.IdeaStatusChanged, HandleIdeaStatusChanged);

        await _ideaHub.StartAsync();

        NotificationService.Notify(NotificationSeverity.Success, "Notifications enabled");
    }

    private async Task HandleNewIdeaAdded(string authorHandle, string ideaTitle)
    {
        NotificationService.Notify(NotificationSeverity.Info, "New idea added", $"{authorHandle} added a new idea \"{ideaTitle}\"", duration: 10000);
    }
    
    private async Task HandleNewCommentAdded(string ideaId, string ideaTitle)
    {
        NotificationService.Notify(NotificationSeverity.Info, "New comment added", $"New comment has been added to idea \"{ideaTitle}\". Click the notification to navigate to the idea", click: (ns) => HandleNewCommentAddedClick(ideaId), duration: 10000);
    }

    private void HandleNewCommentAddedClick(string ideaId)
    {
        NavigationManager.NavigateTo($"/idea/{ideaId}");
    }
    
    private async Task HandleIdeaStatusChanged(string ideaId, string statusTitle, string ideaTitle)
    {
        NotificationService.Notify(NotificationSeverity.Info, "Idea status changed", $"Idea status changed for \"{ideaTitle}\". New status \"{statusTitle}\"", duration: 10000);
    }
}