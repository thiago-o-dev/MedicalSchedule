namespace Api.Gateway.Models;

public sealed record RegisterRequest(
    string Name,
    string Document,
    string Phone,
    string Email,
    string Password,
    bool IsOwner,
    string? Specialty = null);
