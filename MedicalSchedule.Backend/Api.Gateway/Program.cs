using Microsoft.Extensions.Options;
using Scalar.AspNetCore;
using Yarp.ReverseProxy;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
        policy
            .AllowAnyOrigin() // pq é teste viu
            .AllowAnyHeader()
            .AllowAnyMethod()));

builder.Services.AddControllers();

builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddServiceDiscoveryDestinationResolver();

builder.Services.AddAuthorization();

builder.Services.AddOpenApi();

var keycloakBase =
    builder.Configuration["services:keycloak:https:0"]?.TrimEnd('/')
    ?? builder.Configuration["Keycloak:BaseUrl"]!;

builder.Configuration["Keycloak:BaseUrl"] = keycloakBase;
var realm = builder.Configuration["Keycloak:Realm"]!;
var clientId = builder.Configuration["Keycloak:ClientId"]!;

builder.AddDefaultAuthentication(keycloakBase, realm);

builder.Services.AddAuthorization();

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "MedicalSchedule Gateway";

        options.AddDocument("v1", "Gateway API");

        options.AddDocument(
            "Registry API",
            routePattern: "/registry/openapi/v1.json");
    });
}

app.UseCors();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapReverseProxy();
app.MapControllers();

app.Run();
