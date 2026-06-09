using BuildingBlocks.Messaging.Extensions;
using BuildingBlocks.Persistence.Extensions;
using Caching.Extensions;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Scheduling.Contracts.Events;
using Scheduling.Features.Sagas;
using Scheduling.Infrastructure;
using Scheduling.Infrastructure.Messaging;
using Scheduling.Infrastructure.Persistence;
using SharedKernel.Abstractions;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddKeycloakAuthentication();
builder.AddProblemDetailsHandling();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("scheduling-db")
    ?? throw new InvalidOperationException("Connection string 'scheduling-db' not configured.");

var rabbitMqConnectionString = builder.Configuration.GetConnectionString("rabbitmq")
    ?? throw new InvalidOperationException("Connection string 'rabbitmq' not configured.");

var redisConnectionString = builder.Configuration.GetConnectionString("redis")
    ?? throw new InvalidOperationException("Connection string 'redis' not configured.");

builder.Services.AddPersistence<SchedulingDbContext>(connectionString);
builder.Services.AddSlotLockService(redisConnectionString);
builder.Services.AddMessaging(rabbitMqConnectionString);
builder.Services.AddOutboxWorker<SchedulingDbContext>();

builder.Services.AddScoped<ISchedulingUnitOfWork>(sp => sp.GetRequiredService<SchedulingDbContext>());

builder.Services.Scan(scan => scan
    .FromAssemblyOf<Program>()
    .AddClasses(c => c.AssignableTo(typeof(IQueryHandler<,>)))
        .AsImplementedInterfaces()
        .WithScopedLifetime()
    .AddClasses(c => c.AssignableTo(typeof(ICommandHandler<>)))
        .AsImplementedInterfaces()
        .WithScopedLifetime()
    .AddClasses(c => c.AssignableTo(typeof(ICommandHandler<,>)))
        .AsImplementedInterfaces()
        .WithScopedLifetime());

builder.Services.AddScoped<IDomainEventHandler<PetDeletionRequestedEvent>, PetDeletionRequestedSagaHandler>();
builder.Services.AddHostedService<SchedulingSubscriptionsService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SchedulingDbContext>();
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
