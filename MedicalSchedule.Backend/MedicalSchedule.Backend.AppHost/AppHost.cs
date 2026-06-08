var builder = DistributedApplication.CreateBuilder(args);

var postgresPassword = builder.AddParameter("postgres-password", "keycloak", secret: true);
var keycloakPassword = builder.AddParameter("keycloak-password", "thiagozika", secret: true);

// Databases

var postgres = builder.AddPostgres("postgres", password: postgresPassword)
    .WithDataVolume();

var keycloakDb = postgres.AddDatabase("keycloak-db");
var registryDb = postgres.AddDatabase("registry-db");
var schedulingDb = postgres.AddDatabase("scheduling-db");
var paymentsDb = postgres.AddDatabase("payments-db");

// Infrastructure

var mailhog = builder.AddContainer("mailhog", "mailhog/mailhog")
    .WithEndpoint(port: 1025, targetPort: 1025, name: "smtp")
    .WithEndpoint(port: 8025, targetPort: 8025, name: "http");

var redis = builder.AddRedis("redis");

var rabbitmq = builder.AddRabbitMQ("rabbitmq")
    .WithManagementPlugin()
    .WithDataVolume();

var keycloak = builder.AddKeycloak("keycloak", 8081)
    .WithReference(keycloakDb)
    .WithEnvironment("KEYCLOAK_ADMIN", "pedropedro")
    .WithEnvironment("KEYCLOAK_ADMIN_PASSWORD", keycloakPassword)
    .WaitFor(postgres);

// Services

var registry =
    builder.AddProject<Projects.Registry_Api>("registry")
        .WithReference(rabbitmq)
        .WithReference(registryDb)
        .WaitFor(rabbitmq)
        .WaitFor(registryDb);

var scheduling =
    builder.AddProject<Projects.Scheduling>("scheduling")
        .WithReference(redis)
        .WithReference(rabbitmq)
        .WithReference(schedulingDb)
        .WaitFor(rabbitmq)
        .WaitFor(schedulingDb);

var payments =
    builder.AddProject<Projects.Payments>("payments")
        .WithReference(redis)
        .WithReference(rabbitmq)
        .WithReference(paymentsDb)
        .WaitFor(rabbitmq)
        .WaitFor(paymentsDb);

var notifications =
    builder.AddProject<Projects.Notifications>("notifications")
        .WithReference(rabbitmq)
        .WithReference(registry)
        .WaitFor(rabbitmq)
        .WaitFor(registry);

var whatsapp =
    builder.AddProject<Projects.WhatsappBot>("whatsappbot")
        .WithReference(redis)
        .WithReference(rabbitmq)
        .WaitFor(rabbitmq);

var gateway =
    builder.AddProject<Projects.Api_Gateway>("gateway")
        .WithReference(keycloak)
        .WithReference(registry)
        .WithReference(scheduling)
        .WithReference(payments)
        .WithReference(notifications)
        .WithReference(whatsapp)
        .WaitFor(registry);

builder.Build().Run();
