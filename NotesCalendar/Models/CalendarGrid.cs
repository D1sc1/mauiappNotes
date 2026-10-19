namespace NotesCalendar.Models;

/// <summary>Чистая логика построения сетки месяца (6 недель × 7 дней).</summary>
public static class CalendarGrid
{
    public const int DaysInWeek = 7;
    public const int Weeks = 6;
    public const int CellCount = DaysInWeek * Weeks;

    /// <summary>Дата первой ячейки сетки для месяца, в котором лежит <paramref name="month"/>.</summary>
    public static DateTime GetGridStart(DateTime month, DayOfWeek firstDayOfWeek)
    {
        var firstOfMonth = new DateTime(month.Year, month.Month, 1);
        var offset = ((int)firstOfMonth.DayOfWeek - (int)firstDayOfWeek + DaysInWeek) % DaysInWeek;
        return firstOfMonth.AddDays(-offset);
    }

    /// <summary>Тот же день месяца в другом месяце; если такого дня нет — последний день месяца.</summary>
    public static DateTime ShiftMonth(DateTime date, int months)
    {
        var target = new DateTime(date.Year, date.Month, 1).AddMonths(months);
        var day = Math.Min(date.Day, DateTime.DaysInMonth(target.Year, target.Month));
        return new DateTime(target.Year, target.Month, day);
    }
}
