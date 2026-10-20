using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using NotesCalendar.Models;
using NotesCalendar.Services;

namespace NotesCalendar.ViewModels;

public partial class CalendarViewModel : ObservableObject
{
    private readonly INoteRepository _repository;
    private readonly CultureInfo _culture = CultureInfo.CurrentCulture;
    private readonly DayOfWeek _firstDayOfWeek;

    private List<Note> _visibleNotes = [];
    private DateTime _visibleMonth;
    private int _loadVersion;

    [ObservableProperty]
    private string monthTitle = string.Empty;

    [ObservableProperty]
    private string selectedDateTitle = string.Empty;

    [ObservableProperty]
    private DateTime selectedDate = DateTime.Today;

    [ObservableProperty]
    private bool isMonthPickerOpen;

    [ObservableProperty]
    private int pickerYear;

    public ObservableCollection<CalendarDay> Days { get; } = [];

    public ObservableCollection<MonthItem> PickerMonths { get; } = [];

    public ObservableCollection<Note> SelectedDayNotes { get; } = [];

    public IReadOnlyList<string> WeekDayNames { get; }

    public CalendarViewModel(INoteRepository repository)
    {
        _repository = repository;
        _firstDayOfWeek = _culture.DateTimeFormat.FirstDayOfWeek;
        _visibleMonth = FirstOfMonth(SelectedDate);

        WeekDayNames = Enumerable.Range(0, CalendarGrid.DaysInWeek)
            .Select(i => (DayOfWeek)(((int)_firstDayOfWeek + i) % CalendarGrid.DaysInWeek))
            .Select(d => Capitalize(_culture.DateTimeFormat.GetShortestDayName(d)))
            .ToList();

        for (var i = 0; i < CalendarGrid.CellCount; i++)
            Days.Add(new CalendarDay());

        // После сохранения/удаления заметки переходим на её день; данные подтянет OnAppearing страницы.
        WeakReferenceMessenger.Default.Register<CalendarViewModel, NoteChangedMessage>(
            this, static (recipient, message) => recipient.ShowDate(message.Value));
    }

    [RelayCommand]
    private Task LoadAsync() => ReloadAsync();

    [RelayCommand]
    private Task PreviousMonthAsync() => ChangeMonthAsync(-1);

    [RelayCommand]
    private Task NextMonthAsync() => ChangeMonthAsync(1);

    [RelayCommand]
    private Task PreviousYearAsync() => ChangeMonthAsync(-12);

    [RelayCommand]
    private Task NextYearAsync() => ChangeMonthAsync(12);

    [RelayCommand]
    private void ToggleMonthPicker()
    {
        if (IsMonthPickerOpen)
        {
            IsMonthPickerOpen = false;
            return;
        }

        PickerYear = _visibleMonth.Year;
        BuildPickerMonths();
        IsMonthPickerOpen = true;
    }

    [RelayCommand]
    private void PreviousPickerYear() => ShiftPickerYear(-1);

    [RelayCommand]
    private void NextPickerYear() => ShiftPickerYear(1);

    [RelayCommand]
    private Task PickMonthAsync(MonthItem? month)
    {
        if (month is null)
            return Task.CompletedTask;

        var delta = (PickerYear - SelectedDate.Year) * 12 + month.Number - SelectedDate.Month;
        ShowDate(CalendarGrid.ShiftMonth(SelectedDate, delta));
        return ReloadAsync();
    }

    [RelayCommand]
    private Task GoToTodayAsync()
    {
        ShowDate(DateTime.Today);
        return ReloadAsync();
    }

    [RelayCommand]
    private async Task SelectDayAsync(CalendarDay? day)
    {
        if (day is null)
            return;

        var monthChanged = FirstOfMonth(day.Date) != _visibleMonth;
        ShowDate(day.Date);

        if (monthChanged)
            await ReloadAsync();
        else
            ApplySelection();
    }

    [RelayCommand]
    private Task AddNoteAsync()
    {
        var date = SelectedDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        return Shell.Current.GoToAsync($"{Routes.NoteEdit}?date={date}");
    }

    [RelayCommand]
    private Task EditNoteAsync(Note? note) =>
        note is null ? Task.CompletedTask : Shell.Current.GoToAsync($"{Routes.NoteEdit}?id={note.Id}");

    [RelayCommand]
    private async Task DeleteNoteAsync(Note? note)
    {
        if (note is null)
            return;

        await _repository.DeleteAsync(note);
        await ReloadAsync();
    }

    private Task ChangeMonthAsync(int delta)
    {
        // Выбранный день переезжает вместе с месяцем, чтобы выделение не пропадало.
        var target = CalendarGrid.ShiftMonth(SelectedDate, delta);
        if (target.Year is < CalendarGrid.MinYear or > CalendarGrid.MaxYear)
            return Task.CompletedTask;

        ShowDate(target);
        return ReloadAsync();
    }

    private void ShiftPickerYear(int delta)
    {
        var year = PickerYear + delta;
        if (year is < CalendarGrid.MinYear or > CalendarGrid.MaxYear)
            return;

        PickerYear = year;
        BuildPickerMonths();
    }

    private void BuildPickerMonths()
    {
        var today = DateTime.Today;

        PickerMonths.Clear();
        for (var month = 1; month <= 12; month++)
        {
            PickerMonths.Add(new MonthItem
            {
                Number = month,
                Name = Capitalize(_culture.DateTimeFormat.GetMonthName(month)),
                IsSelected = PickerYear == _visibleMonth.Year && month == _visibleMonth.Month,
                IsToday = PickerYear == today.Year && month == today.Month,
            });
        }
    }

    private void ShowDate(DateTime date)
    {
        SelectedDate = date.Date;
        _visibleMonth = FirstOfMonth(SelectedDate);
        IsMonthPickerOpen = false;
    }

    private async Task ReloadAsync()
    {
        // Быстрые переключения месяцев: учитываем только результат последнего запроса.
        var version = ++_loadVersion;

        var gridStart = CalendarGrid.GetGridStart(_visibleMonth, _firstDayOfWeek);
        var notes = await _repository.GetRangeAsync(gridStart, gridStart.AddDays(CalendarGrid.CellCount));
        if (version != _loadVersion)
            return;

        _visibleNotes = notes;
        var counts = notes.GroupBy(n => n.Date.Date).ToDictionary(g => g.Key, g => g.Count());
        var today = DateTime.Today;

        for (var i = 0; i < Days.Count; i++)
        {
            var date = gridStart.AddDays(i);
            var day = Days[i];
            day.Date = date;
            day.IsCurrentMonth = date.Month == _visibleMonth.Month && date.Year == _visibleMonth.Year;
            day.IsToday = date == today;
            day.NoteCount = counts.GetValueOrDefault(date);
        }

        MonthTitle = Capitalize(_visibleMonth.ToString("MMMM yyyy", _culture));
        ApplySelection();
    }

    private void ApplySelection()
    {
        foreach (var day in Days)
            day.IsSelected = day.Date == SelectedDate;

        SelectedDateTitle = Capitalize(SelectedDate.ToString("dddd, d MMMM yyyy", _culture));

        SelectedDayNotes.Clear();
        foreach (var note in _visibleNotes.Where(n => n.Date.Date == SelectedDate))
            SelectedDayNotes.Add(note);
    }

    private static DateTime FirstOfMonth(DateTime date) => new(date.Year, date.Month, 1);

    private static string Capitalize(string text) =>
        string.IsNullOrEmpty(text) ? text : char.ToUpper(text[0], CultureInfo.CurrentCulture) + text[1..];
}
