using Microsoft.AspNetCore.Mvc.RazorPages;
using SubFlow.Application.Subscriptions.Models;
using SubFlow.Application.Subscriptions.Queries;

namespace SubFlow.Web.Pages.Subscriptions;

public class IndexModel : PageModel
{
    private readonly GetSubscriptionsQuery _query;

    public IndexModel(GetSubscriptionsQuery query)
    {
        _query = query;
    }

    public IReadOnlyList<SubscriptionListItem> Items { get; private set; } = Array.Empty<SubscriptionListItem>();

    public async Task OnGetAsync(CancellationToken ct)
    {
        Items = await _query.ExecuteAsync(ct);
    }
}
