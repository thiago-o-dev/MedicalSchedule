using MedicalSchedule.Application.Features.Consultations.Appointments;
using MedicalSchedule.Application.ViewModels.Consultations;
using MedicalSchedule.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace MedicalSchedule.WebApi.Controllers.Consultations;

[Authorize]
[ApiController]
[Route("api/consultations/appointments")]
public class AppointmentsController(
    ScheduleAppointmentCommandHandler scheduleHandler,
    CancelAppointmentCommandHandler cancelHandler,
    RescheduleAppointmentCommandHandler rescheduleHandler,
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
    [EnableRateLimiting("appointments-write")]
    public async Task<IActionResult> Schedule([FromBody] ScheduleAppointmentViewModel vm, CancellationToken ct)
    {
        var result = await scheduleHandler.HandleAsync(vm, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPatch("{id:guid}/reschedule")]
    [EnableRateLimiting("appointments-write")]
    public async Task<IActionResult> Reschedule(Guid id, [FromBody] RescheduleAppointmentViewModel vm, CancellationToken ct)
        => Ok(await rescheduleHandler.HandleAsync(id, vm, ct));

    [HttpPatch("{id:guid}/cancel")]
    [EnableRateLimiting("appointments-write")]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken ct)
        => Ok(await cancelHandler.HandleAsync(id, ct));
}
