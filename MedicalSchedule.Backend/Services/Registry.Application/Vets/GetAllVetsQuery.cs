namespace Registry.Features.Vets;

public sealed record GetAllVetsQuery;

public sealed record VetResponse(
    Guid Id,
    string Name,
    string Crm,
    string Specialty,
    string Email,
    bool IsActive);
