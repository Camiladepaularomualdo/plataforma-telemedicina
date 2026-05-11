using ClinifyMobile.ViewModels;

namespace ClinifyMobile.Views;

public partial class AddPatientPage : ContentPage
{
    private readonly AddPatientViewModel _viewModel;

    public AddPatientPage(AddPatientViewModel viewModel)
    {
        InitializeComponent();
        _viewModel     = viewModel;
        BindingContext  = viewModel;
    }
}
