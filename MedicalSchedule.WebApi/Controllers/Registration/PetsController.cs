using MedicalSchedule.Application.Features.Registration.Pets;
using MedicalSchedule.Application.ViewModels.Registration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalSchedule.WebApi.Controllers.Registration;

[Authorize]
[ApiController]
[Route("api/registration/pets")]
public class PetsController(
    CreatePetCommandHandler createHandler,
    GetAllPetsQuery getAllQuery,
    GetPetByIdQuery getByIdQuery,
    UpdatePetCommandHandler updateHandler,
    DeactivatePetCommandHandler deactivateHandler) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid? ownerId, CancellationToken ct)
        => Ok(await getAllQuery.HandleAsync(ownerId, ct));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        => Ok(await getByIdQuery.HandleAsync(id, ct));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePetViewModel vm, CancellationToken ct)
    {
        var result = await createHandler.HandleAsync(vm, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePetViewModel vm, CancellationToken ct)
        => Ok(await updateHandler.HandleAsync(id, vm, ct));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken ct)
    {
        await deactivateHandler.HandleAsync(id, ct);
        return NoContent();
    }
}
