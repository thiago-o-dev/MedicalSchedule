using Notifications.Contracts.Events;
using Notifications.Infrastructure.Email;
using Notifications.Infrastructure.External;
using SharedKernel.Abstractions;

namespace Notifications.Features;

public sealed class ConsultationScheduledEmailHandler(
    IEmailService emailService,
    IRegistryClient registryClient) : IDomainEventHandler<ConsultationScheduledEvent>
{
    public async Task HandleAsync(ConsultationScheduledEvent @event, CancellationToken cancellationToken = default)
    {
        var owner = await registryClient.GetOwnerByPetIdAsync(@event.PetId, cancellationToken);
        if (owner is null) return;

        await emailService.SendAsync(
            to: owner.Email,
            subject: "Appointment Confirmed",
            body: BuildBody(owner.Name, @event.ScheduledAt),
            cancellationToken);
    }

    private static string BuildBody(string ownerName, DateTime scheduledAt) => $"""
        <h2>Appointment Confirmed</h2>
        <p>Dear <strong>{ownerName}</strong>,</p>
        <p>Your pet's appointment has been successfully scheduled.</p>
        <p><strong>Date &amp; Time:</strong> {scheduledAt:dddd, MMMM d, yyyy} at {scheduledAt:HH:mm} UTC</p>
        <p>If you need to reschedule or cancel, please contact us at least 24 hours in advance.</p>
        <br/>
        <p>Thank you,<br/>Medical Schedule Team</p>
        """;
}
