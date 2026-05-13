namespace Formica.ApiService.Warehouse.WarehouseFoundation.Domain.Common;

public abstract class EntityLifecycle
{
    protected EntityLifecycle()
    {
        Id = Guid.CreateVersion7();
        CreatedAtUtc = DateTimeOffset.UtcNow;
        UpdatedAtUtc = CreatedAtUtc;
        IsActive = true;
    }

    protected EntityLifecycle(Guid id, DateTimeOffset createdAtUtc, DateTimeOffset updatedAtUtc, bool isActive)
    {
        Id = id;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = updatedAtUtc;
        IsActive = isActive;
    }

    public Guid Id { get; protected init; }

    public DateTimeOffset CreatedAtUtc { get; protected set; }

    public DateTimeOffset UpdatedAtUtc { get; protected set; }

    public bool IsActive { get; protected set; }

    public void Deactivate()
    {
        if (!IsActive)
        {
            return;
        }

        IsActive = false;
        Touch();
    }

    public void Reactivate()
    {
        if (IsActive)
        {
            return;
        }

        IsActive = true;
        Touch();
    }

    protected void Touch()
    {
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }
}
