namespace Scheduling.Domain.Entities;

public class Appointment
{
    // Maybe this on the future would be nice
    //IEnumerable<Guid> VetIds;
    //IEnumerable<Guid> PetIds;
    //IEnumerable<Guid> OwnerIds;

    Guid VetId;
    Guid PetId;
    Guid OwnerId;

    DateTime Date;
    TimeSpan EstimatedDuration;
}
