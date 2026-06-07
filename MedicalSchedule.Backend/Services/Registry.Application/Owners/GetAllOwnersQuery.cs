namespace Registry.Features.Owners;

public sealed record GetAllOwnersQuery;

public sealed record OwnerResponse(
    Guid Id,
    string Name,
    string Cpf,
    string Phone,
    string Email,
    bool IsActive);
