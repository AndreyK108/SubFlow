namespace SubFlow.Domain.Entities;

public sealed class Service
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public string Name { get; private set; }

    public string? WebsiteUrl { get; private set; }
    public string? LogoUrl { get; private set; }

    public Service(string name, string? websiteUrl = null, string? logoUrl = null)
    {
        Name = string.IsNullOrWhiteSpace(name) ? throw new ArgumentException("Service name is required.", nameof(name)) : name.Trim();
        WebsiteUrl = websiteUrl;
        LogoUrl = logoUrl;
    }
}
