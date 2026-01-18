using Microsoft.AspNetCore.Mvc.RazorPages;
using SubFlow.Application.Services.Models;
using SubFlow.Application.Services.Queries;

namespace SubFlow.Web.Pages.Services;

public class IndexModel : PageModel
{
    private readonly GetServicesQuery _query;

    public IndexModel(GetServicesQuery query)
    {
        _query = query;
    }

    public IReadOnlyList<ServiceListItem> Items { get; private set; } = Array.Empty<ServiceListItem>();

    public async Task OnGetAsync(CancellationToken ct)
    {
        Items = await _query.ExecuteAsync(ct);
    }
}
