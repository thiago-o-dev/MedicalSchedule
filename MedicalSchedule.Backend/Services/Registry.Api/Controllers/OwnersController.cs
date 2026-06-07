using Microsoft.AspNetCore.Mvc;
using Registry.Api.Requests;
using Registry.Features.Owners;
using SharedKernel.Abstractions;

namespace Registry.Api.Controllers;

[ApiController]
[Route("api/owners")]
public sealed class OwnersController(
    ICommandHandler<CreateOwnerCommand, Guid> createOwner,
    IQueryHandler<GetAllOwnersQuery, IReadOnlyList<OwnerResponse>> getAllOwners,
    IQueryHandler<GetOwnerByIdQuery, OwnerResponse> getOwnerById) : ControllerBase
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

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await getAllOwners.HandleAsync(new GetAllOwnersQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{ownerId:guid}")]
    public async Task<IActionResult> GetById(
        Guid ownerId,
        CancellationToken cancellationToken)
    {
        var result = await getOwnerById.HandleAsync(new GetOwnerByIdQuery(ownerId), cancellationToken);
        return Ok(result);
    }
}
