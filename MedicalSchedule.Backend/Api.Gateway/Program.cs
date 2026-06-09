using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Scalar.AspNetCore;
using Yarp.ReverseProxy;
using Yarp.ReverseProxy.Transforms;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddDefaultAuthentication();

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
    .AddTransforms(builderContext =>
    {
        builderContext.AddRequestTransform(transformContext =>
        {
            var authHeader =
                transformContext.HttpContext.Request.Headers.Authorization.ToString();

            if (!string.IsNullOrWhiteSpace(authHeader))
            {
                transformContext.ProxyRequest.Headers.Authorization =
                    System.Net.Http.Headers.AuthenticationHeaderValue.Parse(authHeader);
            }

            return ValueTask.CompletedTask;
        });
    })
    .AddServiceDiscoveryDestinationResolver();

builder.Services.AddOpenApi();

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
//app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapReverseProxy();
app.MapControllers();

app.Run();
