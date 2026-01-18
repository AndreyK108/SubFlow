using SubFlow.Application.Abstractions;
using SubFlow.Domain.Entities;
using SubFlow.Domain.Enums;
using SubFlow.Domain.Services;

namespace SubFlow.Application.Subscriptions.Commands;

public sealed class CreateSubscriptionCommand
{
    private readonly ISubscriptionRepository _repo;
    private readonly ICurrentUser _currentUser;

    public CreateSubscriptionCommand(ISubscriptionRepository repo, ICurrentUser currentUser)
    {
        _repo = repo;
        _currentUser = currentUser;
    }

    public async Task<Guid> ExecuteAsync(Input input, CancellationToken ct)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        var nextChargeDate = SubscriptionScheduleCalculator.CalculateNextChargeDate(
            startDate: input.StartDate,
            billingPeriod: input.BillingPeriod,
            billingPeriodDays: input.BillingPeriod == BillingPeriod.Custom ? input.BillingPeriodDays : null,
            today: today,
            trialEndDate: input.TrialEndDate,
            endDate: input.EndDate
        );

        var sub = new Subscription(
            ownerUserId: _currentUser.UserId,
            serviceId: input.ServiceId,
            title: input.Title,
            amount: input.Amount,
            currency: input.Currency,
            billingPeriod: input.BillingPeriod,
            nextChargeDate: nextChargeDate, // <-- считаем автоматически
            startDate: input.StartDate,
            autoRenew: input.AutoRenew,
            organizationId: _currentUser.OrganizationId,
            categoryId: input.CategoryId,
            billingPeriodDays: input.BillingPeriodDays,
            endDate: input.EndDate,
            trialEndDate: input.TrialEndDate,
            notes: input.Notes
        );

        await _repo.AddAsync(sub, ct);
        await _repo.SaveChangesAsync(ct);

        return sub.Id;
    }

    public sealed record Input(
        Guid ServiceId,
        Guid? CategoryId,
        string Title,
        decimal Amount,
        string Currency,
        BillingPeriod BillingPeriod,
        int? BillingPeriodDays,
        DateOnly StartDate,
        bool AutoRenew,
        DateOnly? TrialEndDate,
        DateOnly? EndDate,
        string? Notes);
}
