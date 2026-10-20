using Microsoft.Extensions.Logging;
using NotesCalendar.Services;
using NotesCalendar.ViewModels;
using NotesCalendar.Views;

namespace NotesCalendar;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();

        builder.Services.AddSingleton<INoteRepository, NoteRepository>();

        builder.Services.AddTransient<CalendarViewModel>();
        builder.Services.AddTransient<CalendarPage>();
        builder.Services.AddTransient<NoteEditViewModel>();
        builder.Services.AddTransient<NoteEditPage>();
        builder.Services.AddTransient<SearchViewModel>();
        builder.Services.AddTransient<SearchPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
