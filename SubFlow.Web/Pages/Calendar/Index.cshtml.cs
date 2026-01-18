using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SubFlow.Application.Subscriptions.Models;
using SubFlow.Application.Subscriptions.Queries;

namespace SubFlow.Web.Pages.Calendar;

public class IndexModel : PageModel
{
    private readonly GetSubscriptionsQuery _subsQuery;

    public IndexModel(GetSubscriptionsQuery subsQuery)
    {
        _subsQuery = subsQuery;
    }

    [BindProperty(SupportsGet = true)]
    public int? Year { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? Month { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? Day { get; set; }

    public string MonthTitle { get; private set; } = "";
    public string SelectedTitle { get; private set; } = "";

    public int PrevYear { get; private set; }
    public int PrevMonth { get; private set; }
    public int NextYear { get; private set; }
    public int NextMonth { get; private set; }

    public List<CalendarDay> CalendarDays { get; private set; } = new();
    public List<ChargeItem> SelectedItems { get; private set; } = new();

    public async Task OnGetAsync(CancellationToken ct)
    {
        var now = DateTime.Today;
        var y = Year ?? now.Year;
        var m = Month ?? now.Month;

        var firstDay = new DateOnly(y, m, 1);
        var lastDay = firstDay.AddMonths(1).AddDays(-1);

        MonthTitle = firstDay.ToString("MMMM yyyy");

        // prev/next month nav
        var prev = firstDay.AddMonths(-1);
        PrevYear = prev.Year;
        PrevMonth = prev.Month;

        var next = firstDay.AddMonths(1);
        NextYear = next.Year;
        NextMonth = next.Month;

        // data
        var subs = await _subsQuery.ExecuteAsync(ct);

        // строим календарную сетку (ПН..ВС)
        CalendarDays = BuildCalendarGrid(firstDay, subs);

        // выбранный день (по query ?day=)
        var selectedDay = ClampSelectedDay(firstDay, Day);
        SelectedTitle = selectedDay.ToString("dd.MM.yyyy");

        SelectedItems = subs
            .Where(x => x.NextChargeDate == selectedDay)
            .OrderBy(x => x.Title)
            .Select(x => new ChargeItem(
                Title: x.Title,
                ServiceName: GuessServiceName(x),
                Amount: x.Amount,
                Currency: x.Currency,
                NextChargeDate: x.NextChargeDate.ToString("dd.MM.yyyy"),
                Status: x.Status.ToString()))
            .ToList();
    }

    private static DateOnly ClampSelectedDay(DateOnly firstDay, int? day)
    {
        var d = day ?? DateTime.Today.Day;
        var last = firstDay.AddMonths(1).AddDays(-1);

        if (d < 1) d = 1;
        if (d > last.Day) d = last.Day;

        return new DateOnly(firstDay.Year, firstDay.Month, d);
    }

    private static List<CalendarDay> BuildCalendarGrid(DateOnly firstDay, IReadOnlyList<SubscriptionListItem> subs)
    {
        var days = new List<CalendarDay>(42);

        // Пн=1 ... Вс=7 (DayOfWeek: Sunday=0)
        int DayOfWeekMon1(DateOnly d)
        {
            var dow = (int)d.DayOfWeek;
            return dow == 0 ? 7 : dow;
        }

        // старт сетки: понедельник недели, в которой первое число месяца
        var startOffset = DayOfWeekMon1(firstDay) - 1;
        var gridStart = firstDay.AddDays(-startOffset);

        var today = DateOnly.FromDateTime(DateTime.Today);
        var inMonth = (int)firstDay.Month;

        // 6 недель * 7 дней = 42
        for (int i = 0; i < 42; i++)
        {
            var date = gridStart.AddDays(i);

            var count = subs.Count(x => x.NextChargeDate == date);

            days.Add(new CalendarDay(
                Date: date,
                IsInMonth: date.Month == inMonth,
                IsToday: date == today,
                Count: count
            ));
        }

        return days;
    }

    // Если в SubscriptionListItem нет ServiceName — оставим пусто/потом улучшим.
    // (У тебя в списке сервисы показываются отдельно, значит это нормальный следующий шаг — добавить ServiceName в query.)
    private static string GuessServiceName(SubscriptionListItem x)
    {
        // если модели пока нет serviceName — не фантазируем
        return "";
    }

    public sealed record CalendarDay(DateOnly Date, bool IsInMonth, bool IsToday, int Count);

    public sealed record ChargeItem(
        string Title,
        string ServiceName,
        decimal Amount,
        string Currency,
        string NextChargeDate,
        string Status);
}
