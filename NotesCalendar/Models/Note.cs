using SQLite;

namespace NotesCalendar.Models;

public class Note
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    /// <summary>День, к которому привязана заметка (только дата, без времени).</summary>
    [Indexed]
    public DateTime Date { get; set; } = DateTime.Today;

    public bool HasTime { get; set; }

    public TimeSpan Time { get; set; }

    /// <summary>Индекс цвета метки в <see cref="NotePalette"/>; 0 — без метки.</summary>
    public int ColorIndex { get; set; }

    [Ignore]
    public string DisplayTitle => string.IsNullOrWhiteSpace(Title) ? "Без названия" : Title;

    [Ignore]
    public string TimeText => HasTime ? Time.ToString(@"hh\:mm") : "Весь день";

    [Ignore]
    public string Preview => Content.Replace('\r', ' ').Replace('\n', ' ').Trim();

    [Ignore]
    public bool HasPreview => Preview.Length > 0;

    /// <summary>Цвет полоски в списке: у заметки без метки полоска прозрачная.</summary>
    [Ignore]
    public Color StripeColor => ColorIndex > 0 ? NotePalette.Get(ColorIndex) : Colors.Transparent;

    [Ignore]
    public string DateTimeText =>
        Date.ToString("d MMM yyyy", System.Globalization.CultureInfo.CurrentCulture) + (HasTime ? ", " + Time.ToString(@"hh\:mm") : string.Empty);
}
