using Notifications.Contracts.Events;
using Notifications.Infrastructure.Email;
using Notifications.Infrastructure.External;
using SharedKernel.Abstractions;

namespace Notifications.Features;

public sealed class ConsultationCancelledEmailHandler(
    IEmailService emailService,
    IRegistryClient registryClient) : IDomainEventHandler<ConsultationCancelledEvent>
{
    public async Task HandleAsync(ConsultationCancelledEvent @event, CancellationToken cancellationToken = default)
    {
        var owner = await registryClient.GetOwnerByPetIdAsync(@event.PetId, cancellationToken);
        if (owner is null) return;

        await emailService.SendAsync(
            to: owner.Email,
            subject: "Appointment Cancelled",
            body: BuildBody(owner.Name, @event.ScheduledAt),
            cancellationToken);
    }

    private static string BuildBody(string ownerName, DateTime scheduledAt) => $"""
        <h2>Appointment Cancelled</h2>
        <p>Dear <strong>{ownerName}</strong>,</p>
        <p>The appointment scheduled for
        <strong>{scheduledAt:dddd, MMMM d, yyyy} at {scheduledAt:HH:mm} UTC</strong> has been cancelled.</p>
        <p>Please contact us to schedule a new appointment.</p>
        <br/>
        <p>Thank you,<br/>Medical Schedule Team</p>
        """;
}
