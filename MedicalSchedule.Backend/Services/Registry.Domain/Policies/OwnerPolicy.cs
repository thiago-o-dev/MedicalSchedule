using Registry.Domain.Entities;
using SharedKernel.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Registry.Domain.Policies;

public static class OwnerPolicy
{
    public static void EnsureCanCreateOwner(Owner owner)
    {
        if (owner.Cpf.Length != 11)
        {
            throw new DomainValidationException("Invalid CPF length, CPF must be 11 characters");
        }

        if (owner.Email.Split("@").Length != 2)
        {
            throw new DomainValidationException("Email must have a single domain separator");
        }
    }
}
