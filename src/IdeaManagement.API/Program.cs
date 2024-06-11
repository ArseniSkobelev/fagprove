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

    // add mongodb to di container
    builder.Services.AddSingleton(new MongoClient(new MongoUrl(builder.Configuration["mongodb_connection_string"] ?? throw new Exception("No MongoDB connection string found"))));
}

var app = builder.Build();
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseRouting();
    app.UseCors("SpaCorsPolicy");
    
    app.UseAuthentication();  
    app.UseAuthorization();
    
    app.MapControllers();
    app.UseHttpsRedirection();

    // map signalr hubs
    
    app.Run();
}
