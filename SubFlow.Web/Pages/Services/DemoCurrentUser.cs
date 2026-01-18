using SubFlow.Application.Abstractions;

namespace SubFlow.Web.Pages.Services;

public sealed class DemoCurrentUser : ICurrentUser
{
    // один “демо пользователь”, чтобы уже сейчас сохранять OwnerUserId
    public Guid UserId { get; } = Guid.Parse("11111111-1111-1111-1111-111111111111");

    public Guid? OrganizationId => null;
}
