using Registry.Domain.Entities;
using SharedKernel.Exceptions;

namespace MedicalSchedule.Backend.UnitTests.Registry;

public class OwnerDomainTests
{
    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        var owner = Owner.Create("Joao", "12345678900", "+5511999999999", "joao@test.com");

        owner.Id.Should().NotBe(Guid.Empty);
        owner.Cpf.Should().Be("12345678900");
        owner.IsActive.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithBlankCpf_ShouldThrow(string cpf)
    {
        var act = () => Owner.Create("Joao", cpf, "+5511999999999", "joao@test.com");

        act.Should().Throw<DomainValidationException>()
            .WithMessage("*CPF*");
    }
}
