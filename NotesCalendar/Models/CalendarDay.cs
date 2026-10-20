using CommunityToolkit.Mvvm.ComponentModel;

namespace NotesCalendar.Models;

/// <summary>Одна ячейка сетки календаря. Объекты переиспользуются при смене месяца.</summary>
public partial class CalendarDay : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DayNumber))]
    private DateTime date;

    [ObservableProperty]
    private bool isCurrentMonth;

    [ObservableProperty]
    private bool isToday;

    [ObservableProperty]
    private bool isSelected;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasNotes))]
    private int noteCount;

    /// <summary>Цвета точек под числом (до трёх): метки заметок этого дня.</summary>
    [ObservableProperty]
    private IReadOnlyList<Color> marks = [];

    public int DayNumber => Date.Day;

    public bool HasNotes => NoteCount > 0;
}
