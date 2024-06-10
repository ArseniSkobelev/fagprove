using IdeaManagement.WebUI;
using IdeaManagement.WebUI.Domain.Authentication;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<CustomAuthorizationMessageHandler>();

builder.Services.AddHttpClient("ServerAPI", client => client.BaseAddress = new Uri(builder.Configuration["restapi_uri"] ?? throw new Exception("Missing REST API uri configuration")))
    .AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

builder.Services.AddTransient(sp => 
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("ServerAPI"));

builder.Services.AddRadzenComponents();

builder.Services.AddOidcAuthentication(options =>
{
    options.ProviderOptions.Authority = builder.Configuration["auth0_authority"] ??
                                        throw new Exception("Missing Auth0 authority configuration");
    options.ProviderOptions.ClientId = builder.Configuration["auth0_clientid"] ??
                                        throw new Exception("Missing Auth0 client id configuration");
    options.ProviderOptions.ResponseType = "code";
    options.ProviderOptions.AdditionalProviderParameters.Add("audience", builder.Configuration["auth0_identifier"] ?? throw new Exception("Missing Auth0 identifier configuration"));
}).AddAccountClaimsPrincipalFactory<ArrayClaimsPrincipalFactory<RemoteUserAccount>>();

await builder.Build().RunAsync();
