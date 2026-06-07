using BuildingBlocks.Messaging.Extensions;
using BuildingBlocks.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;
using Registry.Features.Shared;
using Registry.Infrastructure.Persistence;
using Registry.Infrastructure.Persistence.Repositories;
using Scalar.AspNetCore;
using SharedKernel.Abstractions;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("registry-db")
    ?? throw new InvalidOperationException("Connection string 'registry-db' not configured.");

var rabbitMqConnectionString = builder.Configuration.GetConnectionString("rabbitmq")
    ?? throw new InvalidOperationException("Connection string 'rabbitmq' not configured.");

builder.Services.AddPersistence<RegistryDbContext>(connectionString);
builder.Services.AddMessaging(rabbitMqConnectionString);
builder.Services.AddOutboxWorker<RegistryDbContext>();

builder.Services.AddScoped<IPetRepository, PetRepository>();
builder.Services.AddScoped<IOwnerRepository, OwnerRepository>();
builder.Services.AddScoped<IVetRepository, VetRepository>();

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

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
