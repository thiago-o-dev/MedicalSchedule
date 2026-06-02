namespace MedicalSchedule.Domain.Abstractions;

public abstract class LifeCycleEntity : BaseEntity
{
    public DateTime CreatedAt { get; protected set; }
    public DateTime UpdatedAt { get; protected set; }
    public bool IsActive { get; protected set; } = true;
    public Guid RowVersion { get; protected set; } = Guid.NewGuid();

    public virtual void Deactivate()
    {
        IsActive = false;
        RowVersion = Guid.NewGuid();
        UpdatedAt = DateTime.UtcNow;
    }

    protected void Touch()
    {
        RowVersion = Guid.NewGuid();
        UpdatedAt = DateTime.UtcNow;
    }
}
