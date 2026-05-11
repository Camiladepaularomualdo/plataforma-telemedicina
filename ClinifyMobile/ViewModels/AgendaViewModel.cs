using System.Collections.ObjectModel;
using System.Windows.Input;
using ClinifyMobile.Helpers;
using ClinifyMobile.Models;
using ClinifyMobile.Services;
using ClinifyMobile.ViewModels.Base;

namespace ClinifyMobile.ViewModels;

/// <summary>
/// ViewModel da tela de Agenda (calendário mensal com agendamentos).
/// </summary>
public class AgendaViewModel : BaseViewModel
{
    private readonly AppointmentService _appointmentService;
    private readonly AuthService        _authService;
    private readonly SessionService     _session;

    public AgendaViewModel(
        AppointmentService appointmentService,
        AuthService authService,
        SessionService session)
    {
        _appointmentService = appointmentService;
        _authService        = authService;
        _session            = session;

        Title = "Agenda";

        LoadCommand               = new Command(async () => await LoadAsync());
        PreviousMonthCommand      = new Command(async () => await PreviousMonthAsync());
        NextMonthCommand          = new Command(async () => await NextMonthAsync());
        LogoutCommand             = new Command(DoLogout);
        GoToAddPatientCommand     = new Command(async () => await Shell.Current.GoToAsync("AddPatientPage"));
        GoToAddAppointmentCommand = new Command(async () => await Shell.Current.GoToAsync("AddAppointmentPage"));
        GoToListCommand           = new Command(async () => await Shell.Current.GoToAsync("AppointmentsListPage"));
        RefreshCommand            = new Command(async () => await LoadAsync());
        UpdateStatusCommand       = new Command<Appointment>(async (a) => await UpdateStatusAsync(a));
        SendEmailCommand          = new Command<Appointment>(async (a) => await SendEmailAsync(a));
        GenerateMeetingCommand    = new Command<Appointment>(async (a) => await GenerateMeetingAsync(a));
    }

    // ─── Properties ──────────────────────────────────────────────────────────

    private DateTime _currentDate = DateTime.Now;
    public DateTime CurrentDate
    {
        get => _currentDate;
        set
        {
            SetProperty(ref _currentDate, value);
            OnPropertyChanged(nameof(MonthYearLabel));
        }
    }

    public string MonthYearLabel =>
        CurrentDate.ToString("MMMM yyyy", new System.Globalization.CultureInfo("pt-BR"))
                   .Replace(CurrentDate.ToString("yyyy"), CurrentDate.Year.ToString()).ToUpper();

    private ObservableCollection<DayGroup> _days = [];
    public ObservableCollection<DayGroup> Days
    {
        get => _days;
        set => SetProperty(ref _days, value);
    }

    private bool _isRefreshing;
    public bool IsRefreshing
    {
        get => _isRefreshing;
        set => SetProperty(ref _isRefreshing, value);
    }

    public string DoctorName => _session.DoctorName;

    // ─── Commands ─────────────────────────────────────────────────────────────

    public ICommand LoadCommand               { get; }
    public ICommand PreviousMonthCommand      { get; }
    public ICommand NextMonthCommand          { get; }
    public ICommand LogoutCommand             { get; }
    public ICommand GoToAddPatientCommand     { get; }
    public ICommand GoToAddAppointmentCommand { get; }
    public ICommand GoToListCommand           { get; }
    public ICommand RefreshCommand            { get; }
    public ICommand UpdateStatusCommand       { get; }
    public ICommand SendEmailCommand          { get; }
    public ICommand GenerateMeetingCommand    { get; }

    // ─── Load Logic ──────────────────────────────────────────────────────────

    public async Task LoadAsync()
    {
        await ExecuteAsync(async () =>
        {
            IsRefreshing = true;
            var appointments = await _appointmentService.GetByMonthAsync(
                CurrentDate.Year, CurrentDate.Month);
            BuildDayGroups(appointments);
            IsRefreshing = false;
        });
    }

    private void BuildDayGroups(List<Appointment> appointments)
    {
        var groups = appointments
            .GroupBy(a => a.Date.Date)
            .OrderBy(g => g.Key)
            .Select(g => new DayGroup(
                g.Key,
                g.Key.ToString("dd/MM - dddd", new System.Globalization.CultureInfo("pt-BR")),
                g.OrderBy(a => a.Time).ToList()))
            .ToList();

        Days = new ObservableCollection<DayGroup>(groups);
    }

    // ─── Month Navigation ─────────────────────────────────────────────────────

    private async Task PreviousMonthAsync()
    {
        CurrentDate = new DateTime(CurrentDate.Year, CurrentDate.Month, 1).AddMonths(-1);
        await LoadAsync();
    }

    private async Task NextMonthAsync()
    {
        CurrentDate = new DateTime(CurrentDate.Year, CurrentDate.Month, 1).AddMonths(1);
        await LoadAsync();
    }

    // ─── Status Update ───────────────────────────────────────────────────────

    private async Task UpdateStatusAsync(Appointment appointment)
    {
        var options = StatusHelper.AllStatuses.Select(s => s.Label).ToArray();
        var chosen  = await Shell.Current.DisplayActionSheet(
            $"Status de {appointment.PatientName}", "Cancelar", null, options);

        if (chosen is null or "Cancelar") return;

        var option = StatusHelper.AllStatuses.FirstOrDefault(s => s.Label == chosen);
        if (option is null) return;

        await ExecuteAsync(async () =>
        {
            await _appointmentService.UpdateStatusAsync(appointment.Id, option.Value);
            await LoadAsync(); // Refresh
        });
    }

    // ─── Send Email ──────────────────────────────────────────────────────────

    private async Task SendEmailAsync(Appointment appointment)
    {
        var confirm = await Shell.Current.DisplayAlert(
            "Enviar E-mail",
            $"Enviar link de atendimento para {appointment.PatientName}?",
            "Enviar", "Cancelar");

        if (!confirm) return;

        await ExecuteAsync(async () =>
        {
            await _appointmentService.SendEmailAsync(appointment.Id);
            await Shell.Current.DisplayAlert("Sucesso", "E-mail enviado com sucesso!", "OK");
        });
    }

    private async Task GenerateMeetingAsync(Appointment appointment)
    {
        var confirm = await Shell.Current.DisplayAlert(
            "Gerar Sala",
            $"Gerar sala de atendimento para {appointment.PatientName}?",
            "Gerar", "Cancelar");

        if (!confirm) return;

        await ExecuteAsync(async () =>
        {
            var url = await _appointmentService.GenerateMeetingAsync(appointment.Id);
            if (!string.IsNullOrEmpty(url))
            {
                await Clipboard.SetTextAsync(url);
                var open = await Shell.Current.DisplayAlert(
                    "Sala Gerada", 
                    "O link da sala foi gerado e copiado para a área de transferência.\n\nDeseja entrar na sala agora?", 
                    "Entrar na Sala", "OK");
                
                if (open)
                {
                    await Launcher.OpenAsync(new Uri(url));
                }
            }
            await LoadAsync();
        });
    }

    // ─── Logout ──────────────────────────────────────────────────────────────

    private void DoLogout()
    {
        _authService.Logout();
        Shell.Current.GoToAsync("//LoginPage");
    }
}

/// <summary>Grupo de agendamentos por dia para CollectionView com agrupamento.</summary>
public class DayGroup : List<Appointment>
{
    public DateTime Date     { get; }
    public string   DayLabel { get; }

    public DayGroup(DateTime date, string dayLabel, IEnumerable<Appointment> items)
        : base(items)
    {
        Date     = date;
        DayLabel = dayLabel;
    }
}
