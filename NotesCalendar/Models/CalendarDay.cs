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

    public int DayNumber => Date.Day;

    public bool HasNotes => NoteCount > 0;
}
