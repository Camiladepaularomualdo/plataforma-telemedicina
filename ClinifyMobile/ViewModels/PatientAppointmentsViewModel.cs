using System.Collections.ObjectModel;
using System.Windows.Input;
using ClinifyMobile.Models;
using ClinifyMobile.Services;
using ClinifyMobile.ViewModels.Base;

namespace ClinifyMobile.ViewModels;

public class PatientAppointmentsViewModel : BaseViewModel
{
    private readonly ApiService _api;
    private readonly SessionService _session;
    private readonly AuthService _auth;

    public ObservableCollection<Appointment> Appointments { get; } = new();

    public ICommand LoadAppointmentsCommand { get; }
    public ICommand JoinVideoCommand { get; }
    public ICommand LogoutCommand { get; }

    public PatientAppointmentsViewModel(ApiService api, SessionService session, AuthService auth)
    {
        _api = api;
        _session = session;
        _auth = auth;

        Title = "Portal do Paciente";
        
        LoadAppointmentsCommand = new Command(async () => await LoadAppointmentsAsync());
        JoinVideoCommand = new Command<string>(async (url) => await JoinVideoAsync(url));
        LogoutCommand = new Command(async () => await LogoutAsync());
    }

    public void OnAppearing()
    {
        LoadAppointmentsCommand.Execute(null);
    }

    private async Task LoadAppointmentsAsync()
    {
        if (IsBusy) return;
        
        await ExecuteAsync(async () =>
        {
            var data = await _api.GetAsync<List<Appointment>>($"appointments/patient/{_session.PatientId}");
            if (data != null)
            {
                Appointments.Clear();
                foreach (var apt in data.OrderBy(a => a.Date))
                {
                    Appointments.Add(apt);
                }
            }
        });
    }

    private async Task JoinVideoAsync(string url)
    {
        if (!string.IsNullOrWhiteSpace(url))
        {
            await Launcher.OpenAsync(new Uri(url));
        }
    }

    private async Task LogoutAsync()
    {
        _auth.Logout();
        await Shell.Current.GoToAsync("//LoginPage");
    }
}
