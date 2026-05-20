using ClinifyMobile.Views;

namespace ClinifyMobile;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Rotas de navegação modal/push
        Routing.RegisterRoute(nameof(AddPatientPage), typeof(AddPatientPage));
        Routing.RegisterRoute(nameof(AddAppointmentPage), typeof(AddAppointmentPage));
        Routing.RegisterRoute(nameof(AppointmentsListPage), typeof(AppointmentsListPage));

        // Aliases curtos usados no GoToAsync
        Routing.RegisterRoute("AddPatientPage", typeof(AddPatientPage));
        Routing.RegisterRoute("AddAppointmentPage", typeof(AddAppointmentPage));
        Routing.RegisterRoute("AppointmentsListPage", typeof(AppointmentsListPage));
    }
}
