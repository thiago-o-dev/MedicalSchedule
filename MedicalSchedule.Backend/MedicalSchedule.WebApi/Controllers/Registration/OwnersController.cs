using MedicalSchedule.Application.Features.Registration.Owners;
using MedicalSchedule.Application.ViewModels.Registration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalSchedule.WebApi.Controllers.Registration;

[Authorize]
[ApiController]
[Route("api/registration/owners")]
public class OwnersController(
    CreateOwnerCommandHandler createHandler,
    GetAllOwnersQuery getAllQuery,
    GetOwnerByIdQuery getByIdQuery,
    UpdateOwnerCommandHandler updateHandler,
    DeactivateOwnerCommandHandler deactivateHandler) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok(await getAllQuery.HandleAsync(ct));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        => Ok(await getByIdQuery.HandleAsync(id, ct));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOwnerViewModel vm, CancellationToken ct)
    {
        var result = await createHandler.HandleAsync(vm, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateOwnerViewModel vm, CancellationToken ct)
        => Ok(await updateHandler.HandleAsync(id, vm, ct));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken ct)
    {
        await deactivateHandler.HandleAsync(id, ct);
        return NoContent();
    }
}
