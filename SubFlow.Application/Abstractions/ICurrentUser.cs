namespace SubFlow.Application.Abstractions;

public interface ICurrentUser
{
    Guid UserId { get; }
    Guid? OrganizationId { get; } // на будущее B2B
}
