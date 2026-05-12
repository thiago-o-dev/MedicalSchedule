using MedicalSchedule.Application.Features.Consultations.Vets;
using MedicalSchedule.Application.ViewModels.Consultations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalSchedule.WebApi.Controllers.Consultations;

[Authorize]
[ApiController]
[Route("api/consultations/vets")]
public class VetsController(
    CreateVetCommandHandler createHandler,
    GetAllVetsQuery getAllQuery,
    GetVetByIdQuery getByIdQuery,
    UpdateVetCommandHandler updateHandler) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok(await getAllQuery.HandleAsync(ct));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        => Ok(await getByIdQuery.HandleAsync(id, ct));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateVetViewModel vm, CancellationToken ct)
    {
        var result = await createHandler.HandleAsync(vm, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateVetViewModel vm, CancellationToken ct)
        => Ok(await updateHandler.HandleAsync(id, vm, ct));
}
