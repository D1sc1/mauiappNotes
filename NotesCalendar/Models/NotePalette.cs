namespace NotesCalendar.Models;

/// <summary>Палитра цветных меток. Индекс 0 — «без метки».</summary>
public static class NotePalette
{
    private static readonly (string Name, Color Fill)[] Entries =
    [
        ("Без метки", Color.FromArgb("#C7CBD3")),
        ("Красный", Color.FromArgb("#E5484D")),
        ("Оранжевый", Color.FromArgb("#F76B15")),
        ("Жёлтый", Color.FromArgb("#F5B800")),
        ("Зелёный", Color.FromArgb("#30A46C")),
        ("Синий", Color.FromArgb("#2F6FED")),
        ("Фиолетовый", Color.FromArgb("#8E4EC6")),
    ];

    private static readonly Color UnlabeledMark = Color.FromArgb("#9AA0AB");

    public static int Count => Entries.Length;

    /// <summary>Цвет метки; для неизвестного индекса — как «без метки».</summary>
    public static Color Get(int index) => Entries[IsValid(index) ? index : 0].Fill;

    /// <summary>Цвет точки в календаре: заметки без метки отмечаются серой точкой.</summary>
    public static Color Mark(int index) => index > 0 && IsValid(index) ? Entries[index].Fill : UnlabeledMark;

    public static List<ColorOption> CreateOptions(bool includeNone) =>
        Entries
            .Select((entry, index) => new ColorOption { Index = index, Name = entry.Name, Fill = entry.Fill })
            .Where(option => includeNone || option.Index > 0)
            .ToList();

    private static bool IsValid(int index) => index >= 0 && index < Entries.Length;
}
