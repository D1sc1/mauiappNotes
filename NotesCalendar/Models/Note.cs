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

    [Ignore]
    public string DisplayTitle => string.IsNullOrWhiteSpace(Title) ? "Без названия" : Title;

    [Ignore]
    public string TimeText => HasTime ? Time.ToString(@"hh\:mm") : "Весь день";

    [Ignore]
    public string Preview => Content.Replace('\r', ' ').Replace('\n', ' ').Trim();

    [Ignore]
    public bool HasPreview => Preview.Length > 0;
}
