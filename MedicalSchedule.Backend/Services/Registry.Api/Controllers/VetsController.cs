using Microsoft.AspNetCore.Mvc;
using Registry.Api.Requests;
using Registry.Features.Vets;
using SharedKernel.Abstractions;

namespace Registry.Api.Controllers;

[ApiController]
[Route("api/vets")]
public sealed class VetsController(
    ICommandHandler<CreateVetCommand, Guid> createVet,
    ICommandHandler<DeactivateVetCommand> deactivateVet,
    IQueryHandler<GetAllVetsQuery, IReadOnlyList<VetResponse>> getAllVets,
    IQueryHandler<GetVetByIdQuery, VetResponse> getVetById) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateVetRequest request,
        CancellationToken cancellationToken)
    {
        var id = await createVet.HandleAsync(
            new CreateVetCommand(request.Name, request.Crm, request.Specialty, request.Email),
            cancellationToken);

        return Created($"/api/vets/{id}", new { id });
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await getAllVets.HandleAsync(new GetAllVetsQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{vetId:guid}")]
    public async Task<IActionResult> GetById(
        Guid vetId,
        CancellationToken cancellationToken)
    {
        var result = await getVetById.HandleAsync(new GetVetByIdQuery(vetId), cancellationToken);
        return Ok(result);
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
