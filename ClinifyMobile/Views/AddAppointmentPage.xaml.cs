using ClinifyMobile.ViewModels;

namespace ClinifyMobile.Views;

public partial class AddAppointmentPage : ContentPage
{
    private readonly AddAppointmentViewModel _viewModel;

    public AddAppointmentPage(AddAppointmentViewModel viewModel)
    {
        InitializeComponent();
        _viewModel     = viewModel;
        BindingContext  = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadPatientsAsync();
    }
}
