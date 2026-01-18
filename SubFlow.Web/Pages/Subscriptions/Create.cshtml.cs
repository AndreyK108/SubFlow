using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SubFlow.Application.Services.Queries;
using SubFlow.Application.Subscriptions.Commands;
using SubFlow.Domain.Enums;

namespace SubFlow.Web.Pages.Subscriptions;

public class CreateModel : PageModel
{
    private readonly CreateSubscriptionCommand _command;
    private readonly GetServicesQuery _servicesQuery;

    public CreateModel(CreateSubscriptionCommand command, GetServicesQuery servicesQuery)
    {
        _command = command;
        _servicesQuery = servicesQuery;
    }

    [BindProperty]
    public FormInput Input { get; set; } = new();

    public List<SelectListItem> ServiceOptions { get; private set; } = new();

    // чтобы UI мог показать подсказку "сначала создай сервис"
    public bool NoServices { get; private set; }

    public async Task OnGetAsync(CancellationToken ct)
    {
        await LoadServicesAsync(ct);

        // defaults
        Input.Currency = "BYN";
        Input.Amount = 0;
        Input.BillingPeriod = BillingPeriod.Monthly;
        Input.StartDate = DateOnly.FromDateTime(DateTime.Today);
        Input.AutoRenew = true;

        // если есть сервисы — выберем первый по умолчанию
        var first = ServiceOptions.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.Value));
        if (first != null && Guid.TryParse(first.Value, out var id))
            Input.ServiceId = id;
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        await LoadServicesAsync(ct);

        Validate();

        if (!ModelState.IsValid)
            return Page();

        var cmdInput = new CreateSubscriptionCommand.Input(
            ServiceId: Input.ServiceId,
            CategoryId: null,
            Title: Input.Title.Trim(),
            Amount: Input.Amount,
            Currency: Input.Currency.Trim().ToUpperInvariant(),
            BillingPeriod: Input.BillingPeriod,
            BillingPeriodDays: Input.BillingPeriod == BillingPeriod.Custom ? Input.BillingPeriodDays : null,
            StartDate: Input.StartDate,
            AutoRenew: Input.AutoRenew,
            TrialEndDate: Input.TrialEndDate,
            EndDate: Input.EndDate,
            Notes: Input.Notes
        );

        await _command.ExecuteAsync(cmdInput, ct);
        return RedirectToPage("/Subscriptions/Index");
    }

    private void Validate()
    {
        if (Input.ServiceId == Guid.Empty)
            ModelState.AddModelError(nameof(Input.ServiceId), "Service is required.");

        if (string.IsNullOrWhiteSpace(Input.Title))
            ModelState.AddModelError(nameof(Input.Title), "Title is required.");

        if (Input.Amount < 0)
            ModelState.AddModelError(nameof(Input.Amount), "Amount must be >= 0.");

        if (string.IsNullOrWhiteSpace(Input.Currency) || Input.Currency.Trim().Length != 3)
            ModelState.AddModelError(nameof(Input.Currency), "Currency must be 3 letters (e.g., BYN).");

        if (Input.BillingPeriod == BillingPeriod.Custom)
        {
            if (Input.BillingPeriodDays is null || Input.BillingPeriodDays <= 0)
                ModelState.AddModelError(nameof(Input.BillingPeriodDays), "Days must be > 0 for Custom period.");
        }

        if (Input.TrialEndDate is not null && Input.TrialEndDate < Input.StartDate)
            ModelState.AddModelError(nameof(Input.TrialEndDate), "Trial end date must be on/after Start date.");

        if (Input.EndDate is not null && Input.EndDate < Input.StartDate)
            ModelState.AddModelError(nameof(Input.EndDate), "End date must be on/after Start date.");
    }

    private async Task LoadServicesAsync(CancellationToken ct)
    {
        var services = await _servicesQuery.ExecuteAsync(ct);

        ServiceOptions = services
            .Select(s => new SelectListItem(s.Name, s.Id.ToString()))
            .ToList();

        NoServices = ServiceOptions.Count == 0;

        if (NoServices)
        {
            // показываем пустой селект + подсказку со ссылкой
            ServiceOptions.Add(new SelectListItem("—", ""));
        }
    }

    public sealed class FormInput
    {
        public Guid ServiceId { get; set; }

        public string Title { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "BYN";

        public BillingPeriod BillingPeriod { get; set; } = BillingPeriod.Monthly;
        public int? BillingPeriodDays { get; set; }

        public DateOnly StartDate { get; set; }
        public bool AutoRenew { get; set; }

        public DateOnly? TrialEndDate { get; set; }
        public DateOnly? EndDate { get; set; }

        public string? Notes { get; set; }
    }
}
