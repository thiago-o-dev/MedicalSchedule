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

var redis = builder.AddRedis("redis");

var rabbitmq = builder.AddRabbitMQ("rabbitmq")
    .WithManagementPlugin();

var keycloak = builder.AddKeycloak("keycloak", 8081)
    .WithReference(keycloakDb)
    .WithEnvironment("KEYCLOAK_ADMIN", "pedropedro")
    .WithEnvironment("KEYCLOAK_ADMIN_PASSWORD", keycloakPassword)
    .WaitFor(postgres);

// Services

var registry =
    builder.AddProject<Projects.Registry>("registry")
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
        .WaitFor(rabbitmq);

var whatsapp =
    builder.AddProject<Projects.WhatsappBot>("whatsappbot")
        .WithReference(redis)
        .WithReference(rabbitmq)
        .WaitFor(rabbitmq);

var gateway =
    builder.AddProject<Projects.Api_Gateway>("gateway")
        .WithReference(registry)
        .WithReference(scheduling)
        .WithReference(payments)
        .WithReference(notifications)
        .WithReference(whatsapp);

builder.Build().Run();
