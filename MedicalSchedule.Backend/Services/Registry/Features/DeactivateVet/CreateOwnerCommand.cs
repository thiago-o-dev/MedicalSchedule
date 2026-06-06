using MediatR;

namespace Registry.Features.CreateOwner;

public sealed record RegisterPetCommand(string Name, string Cpf, string Email, string Phone) : IRequest<Guid>;