using NotesCalendar.Models;
using SQLite;

namespace NotesCalendar.Services;

public class NoteRepository : INoteRepository
{
    private const string DbFileName = "notes.db3";

    private const SQLiteOpenFlags Flags =
        SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache;

    private readonly SemaphoreSlim _initLock = new(1, 1);
    private SQLiteAsyncConnection? _db;

    private async Task<SQLiteAsyncConnection> GetDbAsync()
    {
        if (_db is not null)
            return _db;

        await _initLock.WaitAsync();
        try
        {
            if (_db is null)
            {
                var db = new SQLiteAsyncConnection(Path.Combine(FileSystem.AppDataDirectory, DbFileName), Flags);
                await db.CreateTableAsync<Note>();
                // Колонка ColorIndex добавляется миграцией sqlite-net; у старых строк в ней NULL.
                await db.ExecuteAsync("UPDATE Note SET ColorIndex = 0 WHERE ColorIndex IS NULL");
                _db = db;
            }
            return _db;
        }
        finally
        {
            _initLock.Release();
        }
    }

    public async Task<List<Note>> GetRangeAsync(DateTime from, DateTime toExclusive)
    {
        var db = await GetDbAsync();
        var notes = await db.Table<Note>()
            .Where(n => n.Date >= from && n.Date < toExclusive)
            .ToListAsync();

        // Сначала заметки на весь день, затем по времени.
        return notes
            .OrderBy(n => n.Date)
            .ThenBy(n => n.HasTime)
            .ThenBy(n => n.Time)
            .ThenBy(n => n.Id)
            .ToList();
    }

    public async Task<List<Note>> GetAllAsync()
    {
        var db = await GetDbAsync();
        var notes = await db.Table<Note>().ToListAsync();

        return notes
            .OrderByDescending(n => n.Date)
            .ThenBy(n => n.HasTime)
            .ThenBy(n => n.Time)
            .ThenBy(n => n.Id)
            .ToList();
    }

    public async Task<Note?> GetAsync(int id)
    {
        var db = await GetDbAsync();
        return await db.Table<Note>().Where(n => n.Id == id).FirstOrDefaultAsync();
    }

    public async Task SaveAsync(Note note)
    {
        var db = await GetDbAsync();
        note.Date = note.Date.Date;

        if (note.Id == 0)
            await db.InsertAsync(note);
        else
            await db.UpdateAsync(note);
    }

    public async Task DeleteAsync(Note note)
    {
        var db = await GetDbAsync();
        await db.DeleteAsync(note);
    }
}
