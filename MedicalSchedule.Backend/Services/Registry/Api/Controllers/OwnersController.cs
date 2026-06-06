using Microsoft.AspNetCore.Mvc;
using Registry.Api.Requests;
using Registry.Features.Owners;
using SharedKernel.Abstractions;

namespace Registry.Api.Controllers;

[ApiController]
[Route("api/owners")]
public sealed class OwnersController(
    ICommandHandler<CreateOwnerCommand, Guid> createOwner) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateOwnerRequest request,
        CancellationToken cancellationToken)
    {
        var id = await createOwner.HandleAsync(
            new CreateOwnerCommand(request.Name, request.Cpf, request.Phone, request.Email),
            cancellationToken);

        return Created($"/api/owners/{id}", new { id });
    }
}
