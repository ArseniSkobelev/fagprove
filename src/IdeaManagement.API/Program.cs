using IdeaManagement.API.Domain.Database;
using IdeaManagement.API.Hubs;
using IdeaManagement.API.Repositories;
using IdeaManagement.API.Services;
using IdeaManagement.Shared;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);
{
    // cors configuration
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("SpaCorsPolicy",
            corsBuilder =>
            {
                var spaBaseUri = builder.Configuration["spa_baseuri"];
                corsBuilder.WithOrigins(spaBaseUri)
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
    });
    
    // signalr configuration
    builder.Services.AddSignalR();

    builder.Services.AddResponseCompression(opts =>
    {
        opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
            new[] { "application/octet-stream" });
    });
    
    // auth configuration
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["auth0_authority"];
        options.Audience = builder.Configuration["auth0_identifier"];

        // inject the access_token from the request into the event context
        // ref; https://learn.microsoft.com/en-us/aspnet/core/signalr/authn-and-authz?view=aspnetcore-8.0
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];

                var path = context.HttpContext.Request.Path;

                if (!string.IsNullOrEmpty(accessToken) && (path.StartsWithSegments(Constants.SignalRHubs.Base)))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };
    });
    
    // swagger configuration
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        var jwtSecurityScheme = new OpenApiSecurityScheme
        {
            BearerFormat = "JWT",
            Name = "JWT Authentication",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = JwtBearerDefaults.AuthenticationScheme,

            Reference = new OpenApiReference
            {
                Id = JwtBearerDefaults.AuthenticationScheme,
                Type = ReferenceType.SecurityScheme
            }
        };

        c.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            { jwtSecurityScheme, Array.Empty<string>() }
        });

        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1",
            Title = "Idea management software REST API",
            Description = "A REST API for an idea management software"
        });
    });
    
    builder.Services.AddControllers();
    
    // add service and repos
    builder.Services.AddScoped<IStatusRepository, StatusRepository>();
    builder.Services.AddScoped<IStatusService, StatusService>();
    builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
    builder.Services.AddScoped<ICategoryService, CategoryService>();
    builder.Services.AddScoped<IIdeasRepository, IdeasRepository>();
    builder.Services.AddScoped<IIdeasService, IdeasService>();
    builder.Services.AddScoped<IAuth0Service, Auth0Service>();
    builder.Services.AddScoped<ICommentRepository, CommentRepository>();

    // add mongodb to di container
    builder.Services.AddSingleton(new MongoClient(new MongoUrl(builder.Configuration["mongodb_connection_string"] ?? throw new Exception("No MongoDB connection string found"))).GetDatabase("IdeaManagement"));
}

var app = builder.Build();
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    
    // seed db
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<IMongoDatabase>();
        await DatabaseSeeder.SeedAsync(context);
    }

    app.UseRouting();
    app.UseCors("SpaCorsPolicy");
    
    app.UseAuthentication();  
    app.UseAuthorization();
    
    app.MapControllers();
    app.UseHttpsRedirection();

    // map signalr hubs
    app.MapHub<IdeaHub>(Constants.SignalRHubs.Ideas);
    app.MapHub<Auth0Hub>(Constants.SignalRHubs.Auth0);

    app.MapGet("/health", () => "Healthy");
    
    app.Run();
}
