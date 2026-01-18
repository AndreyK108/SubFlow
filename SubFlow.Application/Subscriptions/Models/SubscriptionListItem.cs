using SubFlow.Domain.Enums;

namespace SubFlow.Application.Subscriptions.Models;

public sealed record SubscriptionListItem(
    Guid Id,
    string Title,
    decimal Amount,
    string Currency,
    BillingPeriod BillingPeriod,
    DateOnly NextChargeDate,
    SubscriptionStatus Status);
