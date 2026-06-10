using Registry.Domain.Policies;
using SharedKernel.Abstractions;
using SharedKernel.Exceptions;

namespace Registry.Domain.Entities;

public class Owner : LifeCycleEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Cpf { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;

    private Owner() { }

    public static Owner Create(string name, string cpf, string phone, string email)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainValidationException("Owner name is required.");
        if (string.IsNullOrWhiteSpace(cpf))
            throw new DomainValidationException("CPF is required.");
        if (string.IsNullOrWhiteSpace(phone))
            throw new DomainValidationException("Phone is required.");
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainValidationException("Email is required.");


        var owner = new Owner
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Cpf = cpf.Trim(),
            Phone = phone.Trim(),
            Email = email.Trim(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        OwnerPolicy.EnsureCanCreateOwner(owner);

        // owner.RaiseDomainEvent(new OwnerCreatedDomainEvent(owner.Id));

        return owner;
    }

    public void Update(string name, string phone, string email)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainValidationException("Owner name is required.");

        Name = name.Trim();
        Phone = phone.Trim();
        Email = email.Trim();
        Touch();
    }
}
