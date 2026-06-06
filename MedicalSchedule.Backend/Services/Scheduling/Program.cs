using BuildingBlocks.Messaging.Extensions;
using BuildingBlocks.Persistence.Extensions;
using Scheduling.Infrastructure;
using Scheduling.Infrastructure.Persistence;
using SharedKernel.Abstractions;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("scheduling-db")
    ?? throw new InvalidOperationException("Connection string 'scheduling-db' not configured.");

var rabbitMqConnectionString = builder.Configuration.GetConnectionString("rabbitmq")
    ?? throw new InvalidOperationException("Connection string 'rabbitmq' not configured.");

builder.Services.AddPersistence<SchedulingDbContext>(connectionString);
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

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
