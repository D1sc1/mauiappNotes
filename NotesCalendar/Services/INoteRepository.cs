using NotesCalendar.Models;

namespace NotesCalendar.Services;

public interface INoteRepository
{
    /// <summary>Заметки с датой в диапазоне [from, toExclusive), отсортированные по дню и времени.</summary>
    Task<List<Note>> GetRangeAsync(DateTime from, DateTime toExclusive);

    /// <summary>Все заметки: новые дни сверху, внутри дня — по времени.</summary>
    Task<List<Note>> GetAllAsync();

    Task<Note?> GetAsync(int id);

    Task SaveAsync(Note note);

    Task DeleteAsync(Note note);
}
