using NotesCalendar.Views;

namespace NotesCalendar;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(Routes.NoteEdit, typeof(NoteEditPage));
    }
}
