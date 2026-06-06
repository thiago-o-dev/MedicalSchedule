using MediatR;

namespace Registry.Features.CreateOwner;

public sealed record CreateOwnerCommand(string Name, string Email, string Phone) : IRequest<Guid>;