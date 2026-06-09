using Microsoft.AspNetCore.Authentication.JwtBearer;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
        policy
            .SetIsOriginAllowed(origin =>
            {
                var host = new Uri(origin).Host;
                return host is "localhost" or "127.0.0.1";
            })
            .AllowAnyHeader()
            .AllowAnyMethod()));

builder.Services.AddControllers();

builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddServiceDiscoveryDestinationResolver();

builder.Services.AddOpenApi();

var keycloakBase =
    builder.Configuration["services:keycloak:https:0"]?.TrimEnd('/')
    ?? builder.Configuration["Keycloak:BaseUrl"]!;

builder.Configuration["Keycloak:BaseUrl"] = keycloakBase;

builder.Services.AddHttpClient("keycloak")
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback =
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    });

var realm = builder.Configuration["Keycloak:Realm"]!;
var clientId = builder.Configuration["Keycloak:ClientId"]!;

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = $"{keycloakBase}/realms/{realm}";
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters.ValidateAudience = false;
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapReverseProxy();
app.MapControllers();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.Run();
