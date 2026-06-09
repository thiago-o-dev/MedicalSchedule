using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

using Registry.Application.Events;
using Registry.Features.Pets;
using Registry.Infrastructure.Configuration;
using Registry.Infrastructure.Messaging;
using Registry.Infrastructure.Persistence;
using SharedKernel.Abstractions;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddKeycloakAuthentication();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("registry-db")
    ?? throw new InvalidOperationException("Connection string 'registry-db' not configured.");

var rabbitMqConnectionString = builder.Configuration.GetConnectionString("rabbitmq")
    ?? throw new InvalidOperationException("Connection string 'rabbitmq' not configured.");

builder.Services.AddInfrastructure(connectionString, rabbitMqConnectionString);

builder.Services.Scan(scan => scan
    .FromAssemblyOf<Registry.Features.Owners.GetAllOwnersQueryHandler>()
    .AddClasses(c => c.AssignableTo(typeof(IQueryHandler<,>)))
        .AsImplementedInterfaces()
        .WithScopedLifetime()
    .AddClasses(c => c.AssignableTo(typeof(ICommandHandler<>)))
        .AsImplementedInterfaces()
        .WithScopedLifetime()
    .AddClasses(c => c.AssignableTo(typeof(ICommandHandler<,>)))
        .AsImplementedInterfaces()
        .WithScopedLifetime());

builder.Services.AddScoped<IDomainEventHandler<PetDeletionApprovedEvent>, PetDeletionApprovedEventHandler>();
builder.Services.AddScoped<IDomainEventHandler<PetDeletionRejectedEvent>, PetDeletionRejectedEventHandler>();
builder.Services.AddHostedService<RegistrySagaSubscriptionsService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<RegistryDbContext>();
    await db.Database.MigrateAsync();
}

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
