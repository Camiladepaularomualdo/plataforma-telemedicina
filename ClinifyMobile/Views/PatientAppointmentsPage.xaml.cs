using ClinifyMobile.ViewModels;

namespace ClinifyMobile.Views;

public partial class PatientAppointmentsPage : ContentPage
{
    private readonly PatientAppointmentsViewModel _viewModel;

    public PatientAppointmentsPage(PatientAppointmentsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.OnAppearing();
    }
}
