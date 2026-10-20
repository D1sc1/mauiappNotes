namespace NotesCalendar.Models;

/// <summary>Месяц в панели быстрого выбора месяца и года.</summary>
public class MonthItem
{
    public int Number { get; init; }

    public string Name { get; init; } = string.Empty;

    /// <summary>Месяц, который сейчас открыт в календаре.</summary>
    public bool IsSelected { get; init; }

    /// <summary>Месяц, в котором находится сегодняшний день.</summary>
    public bool IsToday { get; init; }
}
