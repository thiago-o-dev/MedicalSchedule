namespace Registry.Features.Owners;

public sealed record CreateOwnerCommand(string Name, string Cpf, string Phone, string Email);
