using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using NotesCalendar.Models;
using NotesCalendar.Services;

namespace NotesCalendar.ViewModels;

public partial class NoteEditViewModel : ObservableObject, IQueryAttributable
{
    private readonly INoteRepository _repository;
    private Note _note = new();

    [ObservableProperty]
    private string title = string.Empty;

    [ObservableProperty]
    private string content = string.Empty;

    [ObservableProperty]
    private DateTime date = DateTime.Today;

    [ObservableProperty]
    private bool hasTime;

    [ObservableProperty]
    private TimeSpan time = new(9, 0, 0);

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PageTitle))]
    [NotifyPropertyChangedFor(nameof(CanDelete))]
    private bool isNew = true;

    [ObservableProperty]
    private int colorIndex;

    public ObservableCollection<ColorOption> ColorOptions { get; } = new(NotePalette.CreateOptions(includeNone: true));

    public string PageTitle => IsNew ? "Новая заметка" : "Заметка";

    public bool CanDelete => !IsNew;

    public NoteEditViewModel(INoteRepository repository)
    {
        _repository = repository;
        OnColorIndexChanged(ColorIndex);
    }

    partial void OnColorIndexChanged(int value)
    {
        foreach (var option in ColorOptions)
            option.IsSelected = option.Index == value;
    }

    [RelayCommand]
    private void SelectColor(ColorOption? option)
    {
        if (option is not null)
            ColorIndex = option.Index;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("date", out var dateValue)
            && DateTime.TryParseExact(dateValue?.ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var parsedDate))
        {
            Date = parsedDate;
        }

        if (query.TryGetValue("id", out var idValue)
            && int.TryParse(idValue?.ToString(), out var id))
        {
            _ = LoadAsync(id);
        }
    }

    private async Task LoadAsync(int id)
    {
        var note = await _repository.GetAsync(id);
        if (note is null)
            return;

        _note = note;
        Title = note.Title;
        Content = note.Content;
        Date = note.Date;
        HasTime = note.HasTime;
        ColorIndex = note.ColorIndex;
        if (note.HasTime)
            Time = note.Time;
        IsNew = false;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(Title) && string.IsNullOrWhiteSpace(Content))
        {
            await Shell.Current.DisplayAlertAsync("Пустая заметка", "Введите заголовок или текст заметки.", "OK");
            return;
        }

        _note.Title = Title.Trim();
        _note.Content = Content.Trim();
        _note.Date = Date.Date;
        _note.HasTime = HasTime;
        _note.ColorIndex = ColorIndex;
        _note.Time = HasTime ? Time : TimeSpan.Zero;

        await _repository.SaveAsync(_note);

        WeakReferenceMessenger.Default.Send(new NoteChangedMessage(_note.Date));
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        if (IsNew)
            return;

        var confirmed = await Shell.Current.DisplayAlertAsync(
            "Удалить заметку?", "Это действие нельзя отменить.", "Удалить", "Отмена");
        if (!confirmed)
            return;

        await _repository.DeleteAsync(_note);

        WeakReferenceMessenger.Default.Send(new NoteChangedMessage(_note.Date));
        await Shell.Current.GoToAsync("..");
    }
}
