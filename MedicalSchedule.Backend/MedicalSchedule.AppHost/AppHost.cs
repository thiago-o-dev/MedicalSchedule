var builder = DistributedApplication.CreateBuilder(args);

var postgresPassword = builder.AddParameter("postgres-password", "keycloak", secret: true);
var keycloakPassword = builder.AddParameter("keycloak-password", "thiagozika", secret: true);

var postgres = builder.AddPostgres("postgres", password: postgresPassword)
    .WithDataVolume();

var keycloakDb = postgres.AddDatabase("keycloakDb");

var keycloak = builder.AddKeycloak("keycloak", 8081)
    .WithReference(keycloakDb)
    .WithEnvironment("KEYCLOAK_ADMIN", "pedropedro")
    .WithEnvironment("KEYCLOAK_ADMIN_PASSWORD", keycloakPassword)
    .WaitFor(postgres);

var webapi = builder.AddProject<Projects.MedicalSchedule_WebApi>("medicalschedule-webapi")
    .WithReference(keycloak);

builder.Build().Run();