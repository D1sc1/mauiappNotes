using CommunityToolkit.Mvvm.ComponentModel;

namespace NotesCalendar.Models;

/// <summary>Кружок выбора цвета метки (на экране заметки и в фильтре поиска).</summary>
public partial class ColorOption : ObservableObject
{
    public int Index { get; init; }

    public string Name { get; init; } = string.Empty;

    public Color Fill { get; init; } = Colors.Gray;

    [ObservableProperty]
    private bool isSelected;
}
