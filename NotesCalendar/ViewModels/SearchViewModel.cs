using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NotesCalendar.Models;
using NotesCalendar.Services;

namespace NotesCalendar.ViewModels;

public partial class SearchViewModel : ObservableObject
{
    private const string HintText = "Введите текст или выберите цвет метки";
    private const string NotFoundText = "Ничего не найдено";
    private const int DebounceMilliseconds = 250;

    private readonly INoteRepository _repository;
    private int _searchVersion;

    [ObservableProperty]
    private string query = string.Empty;

    /// <summary>Индекс цвета для фильтра; 0 — фильтра по цвету нет.</summary>
    [ObservableProperty]
    private int colorFilter;

    [ObservableProperty]
    private string summary = string.Empty;

    [ObservableProperty]
    private string emptyText = HintText;

    public ObservableCollection<Note> Results { get; } = [];

    public ObservableCollection<ColorOption> ColorOptions { get; } = new(NotePalette.CreateOptions(includeNone: false));

    public SearchViewModel(INoteRepository repository)
    {
        _repository = repository;
    }

    partial void OnQueryChanged(string value) => _ = SearchAsync(debounce: true);

    partial void OnColorFilterChanged(int value)
    {
        foreach (var option in ColorOptions)
            option.IsSelected = option.Index == value;

        _ = SearchAsync(debounce: false);
    }

    [RelayCommand]
    private void ToggleColor(ColorOption? option) =>
        ColorFilter = option is null || option.Index == ColorFilter ? 0 : option.Index;

    [RelayCommand]
    private Task RefreshAsync() => SearchAsync(debounce: false);

    [RelayCommand]
    private Task EditNoteAsync(Note? note) =>
        note is null ? Task.CompletedTask : Shell.Current.GoToAsync($"{Routes.NoteEdit}?id={note.Id}");

    private async Task SearchAsync(bool debounce)
    {
        // Актуален только последний запрос: пока пользователь печатает, старые отбрасываются.
        var version = ++_searchVersion;

        if (debounce)
        {
            await Task.Delay(DebounceMilliseconds);
            if (version != _searchVersion)
                return;
        }

        var terms = (Query ?? string.Empty).Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (terms.Length == 0 && ColorFilter == 0)
        {
            Results.Clear();
            Summary = string.Empty;
            EmptyText = HintText;
            return;
        }

        var notes = await _repository.GetAllAsync();
        if (version != _searchVersion)
            return;

        // Поиск в памяти: LIKE в SQLite не различает регистр только для латиницы, а заметок немного.
        var matches = notes
            .Where(n => ColorFilter == 0 || n.ColorIndex == ColorFilter)
            .Where(n => terms.All(t =>
                n.Title.Contains(t, StringComparison.OrdinalIgnoreCase) ||
                n.Content.Contains(t, StringComparison.OrdinalIgnoreCase)))
            .ToList();

        Results.Clear();
        foreach (var note in matches)
            Results.Add(note);

        Summary = matches.Count > 0 ? $"Найдено: {matches.Count}" : string.Empty;
        EmptyText = NotFoundText;
    }
}
