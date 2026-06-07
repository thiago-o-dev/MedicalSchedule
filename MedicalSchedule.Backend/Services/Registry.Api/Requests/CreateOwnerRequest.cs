namespace Registry.Api.Requests;

public sealed record CreateOwnerRequest(string Name, string Cpf, string Phone, string Email);
