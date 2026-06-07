using BuildingBlocks.Messaging.Extensions;
using Notifications.Contracts.Events;
using Notifications.Features;
using Notifications.Infrastructure.Email;
using Notifications.Infrastructure.External;
using Notifications.Infrastructure.Messaging;
using SharedKernel.Abstractions;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var rabbitMqConnectionString = builder.Configuration.GetConnectionString("rabbitmq")
    ?? throw new InvalidOperationException("Connection string 'rabbitmq' not configured.");

builder.Services.AddMessaging(rabbitMqConnectionString);

builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection(EmailSettings.SectionName));

builder.Services.AddScoped<IEmailService, SmtpEmailService>();

builder.Services.AddHttpClient<IRegistryClient, RegistryClient>(client =>
{
    client.BaseAddress = new Uri("https+http://registry");
});

builder.Services.AddScoped<IDomainEventHandler<ConsultationScheduledEvent>, ConsultationScheduledEmailHandler>();
builder.Services.AddScoped<IDomainEventHandler<ConsultationCancelledEvent>, ConsultationCancelledEmailHandler>();
builder.Services.AddScoped<IDomainEventHandler<ConsultationRescheduledEvent>, ConsultationRescheduledEmailHandler>();

builder.Services.AddHostedService<NotificationSubscriptionsService>();

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
