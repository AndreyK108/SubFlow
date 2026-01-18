using SubFlow.Domain.Enums;

namespace SubFlow.Domain.Entities;

public sealed class Subscription
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public Guid? OrganizationId { get; private set; }
    public Guid OwnerUserId { get; private set; }

    public Guid ServiceId { get; private set; }
    public Guid? CategoryId { get; private set; }

    public string Title { get; private set; }
    public SubscriptionStatus Status { get; private set; } = SubscriptionStatus.Active;

    public decimal Amount { get; private set; }
    public string Currency { get; private set; }

    public BillingPeriod BillingPeriod { get; private set; }
    public int? BillingPeriodDays { get; private set; } // only for Custom

    public DateOnly NextChargeDate { get; private set; }
    public bool AutoRenew { get; private set; }

    public DateOnly StartDate { get; private set; }
    public DateOnly? EndDate { get; private set; }
    public DateOnly? TrialEndDate { get; private set; }

    public string? Notes { get; private set; }

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

    public Subscription(
        Guid ownerUserId,
        Guid serviceId,
        string title,
        decimal amount,
        string currency,
        BillingPeriod billingPeriod,
        DateOnly nextChargeDate,
        DateOnly startDate,
        bool autoRenew = true,
        Guid? organizationId = null,
        Guid? categoryId = null,
        int? billingPeriodDays = null,
        DateOnly? endDate = null,
        DateOnly? trialEndDate = null,
        string? notes = null)
    {
        if (ownerUserId == Guid.Empty) throw new ArgumentException("OwnerUserId is required.", nameof(ownerUserId));
        if (serviceId == Guid.Empty) throw new ArgumentException("ServiceId is required.", nameof(serviceId));
        if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be >= 0.");

        Title = string.IsNullOrWhiteSpace(title) ? throw new ArgumentException("Title is required.", nameof(title)) : title.Trim();
        Currency = string.IsNullOrWhiteSpace(currency) ? throw new ArgumentException("Currency is required.", nameof(currency)) : currency.Trim().ToUpperInvariant();

        if (billingPeriod == BillingPeriod.Custom && (!billingPeriodDays.HasValue || billingPeriodDays <= 0))
            throw new ArgumentException("BillingPeriodDays must be > 0 for Custom billing period.", nameof(billingPeriodDays));

        OwnerUserId = ownerUserId;
        ServiceId = serviceId;
        OrganizationId = organizationId;
        CategoryId = categoryId;

        Amount = amount;
        BillingPeriod = billingPeriod;
        BillingPeriodDays = billingPeriod == BillingPeriod.Custom ? billingPeriodDays : null;

        NextChargeDate = nextChargeDate;
        StartDate = startDate;
        EndDate = endDate;
        TrialEndDate = trialEndDate;

        AutoRenew = autoRenew;
        Notes = notes;
    }

    public void SetStatus(SubscriptionStatus status)
    {
        Status = status;
        Touch();
    }

    private void Touch() => UpdatedAt = DateTime.UtcNow;
}
