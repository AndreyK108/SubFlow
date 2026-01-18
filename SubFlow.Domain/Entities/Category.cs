namespace SubFlow.Domain.Entities;

public sealed class Category
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public Guid? OrganizationId { get; private set; }
    public Guid OwnerUserId { get; private set; }

    public string Name { get; private set; }
    public string? Color { get; private set; }
    public int? SortOrder { get; private set; }

    public Category(Guid ownerUserId, string name, Guid? organizationId = null, string? color = null, int? sortOrder = null)
    {
        if (ownerUserId == Guid.Empty) throw new ArgumentException("OwnerUserId is required.", nameof(ownerUserId));
        OwnerUserId = ownerUserId;

        Name = string.IsNullOrWhiteSpace(name) ? throw new ArgumentException("Category name is required.", nameof(name)) : name.Trim();

        OrganizationId = organizationId;
        Color = color;
        SortOrder = sortOrder;
    }
}
