using SharedKernel.Abstractions;
using SharedKernel.Exceptions;

namespace Registry.Domain.Entities;

public class Vet : LifeCycleEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Crm { get; private set; } = string.Empty;
    public string Specialty { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;

    private Vet() { }

    public static Vet Create(string name, string crm, string specialty)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainValidationException("Vet name is required.");
        if (string.IsNullOrWhiteSpace(crm))
            throw new DomainValidationException("CRM is required.");
        if (string.IsNullOrWhiteSpace(specialty))
            throw new DomainValidationException("Specialty is required.");

        return new Vet
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Crm = crm.Trim().ToUpperInvariant(),
            Specialty = specialty.Trim(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void Update(string name, string specialty)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainValidationException("Vet name is required.");

        Name = name.Trim();
        Specialty = specialty.Trim();
        Touch();
    }
}
