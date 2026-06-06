using Microsoft.AspNetCore.Mvc;
using Scheduling.Api.Requests;
using Scheduling.Domain.Enums;
using Scheduling.Features.Consultations;
using SharedKernel.Abstractions;

namespace Scheduling.Api.Controllers;

[ApiController]
[Route("api/consultations")]
public sealed class ConsultationsController(
    ICommandHandler<ScheduleConsultationCommand, Guid> scheduleConsultation,
    ICommandHandler<CancelConsultationCommand> cancelConsultation,
    IQueryHandler<GetAllConsultationsQuery, IReadOnlyList<ConsultationResponse>> getAllConsultations,
    IQueryHandler<GetConsultationByIdQuery, ConsultationResponse> getConsultationById) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Schedule(
        [FromBody] ScheduleConsultationRequest request,
        CancellationToken cancellationToken)
    {
        var id = await scheduleConsultation.HandleAsync(
            new ScheduleConsultationCommand(request.PetId, request.VetId, request.ScheduledAt, request.Notes),
            cancellationToken);

        return Created($"/api/consultations/{id}", new { id });
    }

    [HttpDelete("{consultationId:guid}")]
    public async Task<IActionResult> Cancel(
        Guid consultationId,
        CancellationToken cancellationToken)
    {
        await cancelConsultation.HandleAsync(new CancelConsultationCommand(consultationId), cancellationToken);
        return NoContent();
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? petId,
        [FromQuery] Guid? vetId,
        [FromQuery] ConsultationStatus? status,
        CancellationToken cancellationToken)
    {
        var result = await getAllConsultations.HandleAsync(
            new GetAllConsultationsQuery(petId, vetId, status),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("{consultationId:guid}")]
    public async Task<IActionResult> GetById(
        Guid consultationId,
        CancellationToken cancellationToken)
    {
        var result = await getConsultationById.HandleAsync(
            new GetConsultationByIdQuery(consultationId),
            cancellationToken);

        return Ok(result);
    }
}
