using Notifications.Contracts.Events;
using Notifications.Infrastructure.Email;
using Notifications.Infrastructure.External;
using SharedKernel.Abstractions;

namespace Notifications.Features;

public sealed class ConsultationRescheduledEmailHandler(
    IEmailService emailService,
    IRegistryClient registryClient) : IDomainEventHandler<ConsultationRescheduledEvent>
{
    public async Task HandleAsync(ConsultationRescheduledEvent @event, CancellationToken cancellationToken = default)
    {
        var owner = await registryClient.GetOwnerByPetIdAsync(@event.PetId, cancellationToken);
        if (owner is null) return;

        await emailService.SendAsync(
            to: owner.Email,
            subject: "Appointment Rescheduled",
            body: BuildBody(owner.Name, @event.PreviousScheduledAt, @event.NewScheduledAt),
            cancellationToken);
    }

    private static string BuildBody(string ownerName, DateTime previous, DateTime newDate) => $"""
        <h2>Appointment Rescheduled</h2>
        <p>Dear <strong>{ownerName}</strong>,</p>
        <p>Your pet's appointment has been rescheduled.</p>
        <p><strong>Previous date:</strong> {previous:dddd, MMMM d, yyyy} at {previous:HH:mm} UTC</p>
        <p><strong>New date:</strong> {newDate:dddd, MMMM d, yyyy} at {newDate:HH:mm} UTC</p>
        <p>If you need to make further changes, please contact us at least 24 hours in advance.</p>
        <br/>
        <p>Thank you,<br/>Medical Schedule Team</p>
        """;
}
