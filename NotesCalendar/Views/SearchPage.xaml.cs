using NotesCalendar.ViewModels;

namespace NotesCalendar.Views;

public partial class SearchPage : ContentPage
{
    private readonly SearchViewModel _viewModel;
    private bool _focused;

    public SearchPage(SearchViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // После возврата с экрана заметки результаты могут измениться.
        _viewModel.RefreshCommand.Execute(null);

        // Клавиатуру открываем только при первом показе, а не после возврата из заметки.
        if (!_focused)
        {
            _focused = true;
            SearchBox.Focus();
        }
    }
}
