using ClinifyMobile.ViewModels;

namespace ClinifyMobile.Views;

public partial class AgendaPage : ContentPage
{
    private readonly AgendaViewModel _viewModel;

    public AgendaPage(AgendaViewModel viewModel)
    {
        InitializeComponent();
        _viewModel     = viewModel;
        BindingContext  = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Garante que IsBusy não esteja travado de uma operação anterior,
        // o que causaria o LoadAsync ser silenciosamente ignorado.
        if (_viewModel.IsBusy)
            _viewModel.IsBusy = false;

        await _viewModel.LoadAsync();
    }
}
