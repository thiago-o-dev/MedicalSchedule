using Microsoft.AspNetCore.Mvc;
using Registry.Api.Requests;
using Registry.Features.Vets;
using SharedKernel.Abstractions;

namespace Registry.Api.Controllers;

[ApiController]
[Route("api/vets")]
public sealed class VetsController(
    ICommandHandler<CreateVetCommand, Guid> createVet,
    ICommandHandler<DeactivateVetCommand> deactivateVet) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateVetRequest request,
        CancellationToken cancellationToken)
    {
        var id = await createVet.HandleAsync(
            new CreateVetCommand(request.Name, request.Crm, request.Specialty),
            cancellationToken);

        return Created($"/api/vets/{id}", new { id });
    }

    [HttpDelete("{vetId:guid}")]
    public async Task<IActionResult> Deactivate(
        Guid vetId,
        CancellationToken cancellationToken)
    {
        await deactivateVet.HandleAsync(new DeactivateVetCommand(vetId), cancellationToken);

        return NoContent();
    }
}
