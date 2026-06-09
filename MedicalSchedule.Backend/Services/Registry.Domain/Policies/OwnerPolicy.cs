using Registry.Domain.Entities;
using SharedKernel.Exceptions;
using Registry.Domain.Exceptions;

namespace Registry.Domain.Policies;

public static class OwnerPolicy
{
    public static void EnsureCanCreateOwner(
        Owner owner
        )
    {
        if (owner.Cpf.Length != 11)
        {
            throw new DomainValidationException("Cpf must be 11 character long");
        }

        if (owner.Email.Split("@").Length != 2)
        {
            throw new DomainValidationException("Owner must have single domain separator");
        }
    }
}