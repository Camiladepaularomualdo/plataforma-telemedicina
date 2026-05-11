using ClinifyMobile.ViewModels;

namespace ClinifyMobile.Views;

public partial class AppointmentsListPage : ContentPage
{
    private readonly AppointmentsListViewModel _viewModel;

    public AppointmentsListPage(AppointmentsListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel     = viewModel;
        BindingContext  = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadAsync();
    }
}
