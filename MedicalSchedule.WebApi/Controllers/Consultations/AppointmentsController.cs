using MedicalSchedule.Application.Features.Consultations.Appointments;
using MedicalSchedule.Application.ViewModels.Consultations;
using MedicalSchedule.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalSchedule.WebApi.Controllers.Consultations;

[Authorize]
[ApiController]
[Route("api/consultations/appointments")]
public class AppointmentsController(
    ScheduleAppointmentCommandHandler scheduleHandler,
    CancelAppointmentCommandHandler cancelHandler,
    GetAllAppointmentsQuery getAllQuery,
    GetAppointmentByIdQuery getByIdQuery) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? petId,
        [FromQuery] Guid? vetId,
        [FromQuery] ConsultationStatus? status,
        CancellationToken ct)
        => Ok(await getAllQuery.HandleAsync(petId, vetId, status, ct));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        => Ok(await getByIdQuery.HandleAsync(id, ct));

    [HttpPost]
    public async Task<IActionResult> Schedule([FromBody] ScheduleAppointmentViewModel vm, CancellationToken ct)
    {
        var result = await scheduleHandler.HandleAsync(vm, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPatch("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken ct)
        => Ok(await cancelHandler.HandleAsync(id, ct));
}
