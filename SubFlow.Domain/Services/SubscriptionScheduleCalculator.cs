using SubFlow.Domain.Enums;

namespace SubFlow.Domain.Services;

public static class SubscriptionScheduleCalculator
{
    /// <summary>
    /// Hybrid strategy:
    /// 1) Если trialEndDate есть и он >= today — следующее списание = trialEndDate
    /// 2) Если подписка ещё не началась (startDate >= today) — следующее списание = startDate
    /// 3) Иначе — берём startDate и "накатываем" период, пока не получим дату >= today
    /// </summary>
    public static DateOnly CalculateNextChargeDate(
        DateOnly startDate,
        BillingPeriod billingPeriod,
        int? billingPeriodDays,
        DateOnly today,
        DateOnly? trialEndDate,
        DateOnly? endDate)
    {
        // Trial overrides if it's still active
        if (trialEndDate.HasValue && trialEndDate.Value >= today)
        {
            EnsureNotAfterEnd(trialEndDate.Value, endDate);
            return trialEndDate.Value;
        }

        // Not started yet
        if (startDate >= today)
        {
            EnsureNotAfterEnd(startDate, endDate);
            return startDate;
        }

        // Started already -> roll forward
        var next = startDate;

        for (var i = 0; i < 5000; i++)
        {
            next = AddPeriod(next, billingPeriod, billingPeriodDays);

            if (next >= today)
            {
                EnsureNotAfterEnd(next, endDate);
                return next;
            }

            if (endDate.HasValue && next > endDate.Value)
                throw new ArgumentException("End date is earlier than the calculated next charge date.");
        }

        throw new InvalidOperationException("Unable to calculate next charge date (too many iterations).");
    }

    private static DateOnly AddPeriod(DateOnly date, BillingPeriod billingPeriod, int? billingPeriodDays)
    {
        return billingPeriod switch
        {
            BillingPeriod.Monthly => date.AddMonths(1),
            BillingPeriod.Custom => date.AddDays(ValidateCustomDays(billingPeriodDays)),

            // если в будущем появятся новые значения — лучше явно поддержать,
            // а пока пусть будет понятная ошибка
            _ => throw new ArgumentOutOfRangeException(nameof(billingPeriod), billingPeriod, "Unsupported billing period.")
        };
    }

    private static int ValidateCustomDays(int? days)
    {
        if (!days.HasValue || days.Value <= 0)
            throw new ArgumentException("BillingPeriodDays must be > 0 for Custom billing period.");

        return days.Value;
    }

    private static void EnsureNotAfterEnd(DateOnly next, DateOnly? endDate)
    {
        if (endDate.HasValue && next > endDate.Value)
            throw new ArgumentException("End date is earlier than the calculated next charge date.");
    }
}
