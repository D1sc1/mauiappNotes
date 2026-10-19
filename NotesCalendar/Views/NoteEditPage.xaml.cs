using NotesCalendar.ViewModels;

namespace NotesCalendar.Views;

public partial class NoteEditPage : ContentPage
{
    public NoteEditPage(NoteEditViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
