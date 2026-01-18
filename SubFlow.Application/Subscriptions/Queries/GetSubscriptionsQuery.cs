using SubFlow.Application.Abstractions;
using SubFlow.Application.Subscriptions.Models;

namespace SubFlow.Application.Subscriptions.Queries;

public sealed class GetSubscriptionsQuery
{
    private readonly ISubscriptionRepository _repo;
    private readonly ICurrentUser _currentUser;

    public GetSubscriptionsQuery(ISubscriptionRepository repo, ICurrentUser currentUser)
    {
        _repo = repo;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<SubscriptionListItem>> ExecuteAsync(CancellationToken ct)
    {
        var subs = await _repo.GetByOwnerAsync(_currentUser.UserId, ct);

        return subs
            .OrderByDescending(x => x.UpdatedAt)
            .Select(x => new SubscriptionListItem(
                x.Id,
                x.Title,
                x.Amount,
                x.Currency,
                x.BillingPeriod,
                x.NextChargeDate,
                x.Status))
            .ToList();
    }
}
