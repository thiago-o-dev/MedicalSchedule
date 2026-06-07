namespace Registry.Features.Vets;

public sealed record CreateVetCommand(string Name, string Crm, string Specialty);
