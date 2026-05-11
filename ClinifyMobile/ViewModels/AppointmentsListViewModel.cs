using System.Collections.ObjectModel;
using System.Windows.Input;
using ClinifyMobile.Helpers;
using ClinifyMobile.Models;
using ClinifyMobile.Services;
using ClinifyMobile.ViewModels.Base;

namespace ClinifyMobile.ViewModels;

/// <summary>
/// ViewModel da listagem completa de agendamentos com filtros.
/// Equivale ao AppointmentsListComponent do Angular.
/// </summary>
public class AppointmentsListViewModel : BaseViewModel
{
    private readonly AppointmentService _appointmentService;
    private List<Appointment> _allAppointments = [];

    public AppointmentsListViewModel(AppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
        Title = "Consultas";

        LoadCommand         = new Command(async () => await LoadAsync());
        RefreshCommand      = new Command(async () => await LoadAsync());
        ClearFiltersCommand = new Command(ClearFilters);
        UpdateStatusCommand = new Command<Appointment>(async (a) => await UpdateStatusAsync(a));
        SendEmailCommand    = new Command<Appointment>(async (a) => await SendEmailAsync(a));
        GoBackCommand       = new Command(async () => await Shell.Current.GoToAsync(".."));

        StatusOptions = new ObservableCollection<StatusOption>(
            new[] { new StatusOption((AppointmentStatus)(-1), "Todos") }
                .Concat(StatusHelper.AllStatuses));
        SelectedStatusOption = StatusOptions[0];
    }

    // ─── Properties ──────────────────────────────────────────────────────────

    private ObservableCollection<Appointment> _appointments = [];
    public ObservableCollection<Appointment> Appointments
    {
        get => _appointments;
        set => SetProperty(ref _appointments, value);
    }

    private string _searchName = string.Empty;
    public string SearchName
    {
        get => _searchName;
        set
        {
            SetProperty(ref _searchName, value);
            ApplyFilters();
        }
    }

    private StatusOption? _selectedStatusOption;
    public StatusOption? SelectedStatusOption
    {
        get => _selectedStatusOption;
        set
        {
            SetProperty(ref _selectedStatusOption, value);
            ApplyFilters();
        }
    }

    private bool _isRefreshing;
    public bool IsRefreshing
    {
        get => _isRefreshing;
        set => SetProperty(ref _isRefreshing, value);
    }

    public ObservableCollection<StatusOption> StatusOptions { get; }

    // ─── Commands ─────────────────────────────────────────────────────────────

    public ICommand LoadCommand         { get; }
    public ICommand RefreshCommand      { get; }
    public ICommand ClearFiltersCommand { get; }
    public ICommand UpdateStatusCommand { get; }
    public ICommand SendEmailCommand    { get; }
    public ICommand GoBackCommand       { get; }

    // ─── Logic ───────────────────────────────────────────────────────────────

    public async Task LoadAsync()
    {
        await ExecuteAsync(async () =>
        {
            IsRefreshing = true;
            _allAppointments = await _appointmentService.GetAllAsync();
            ApplyFilters();
            IsRefreshing = false;
        });
    }

    private void ApplyFilters()
    {
        var filtered = _allAppointments.AsEnumerable();

        // Filtro por status
        if (SelectedStatusOption is { } opt && (int)opt.Value >= 0)
            filtered = filtered.Where(a => a.Status == opt.Value);

        // Filtro por nome
        if (!string.IsNullOrWhiteSpace(SearchName))
            filtered = filtered.Where(a =>
                a.PatientName.Contains(SearchName.Trim(), StringComparison.OrdinalIgnoreCase));

        Appointments = new ObservableCollection<Appointment>(
            filtered.OrderByDescending(a => a.Date).ThenBy(a => a.Time));
    }

    private void ClearFilters()
    {
        SearchName           = string.Empty;
        SelectedStatusOption = StatusOptions[0];
    }

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
            await LoadAsync();
        });
    }

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
            await Shell.Current.DisplayAlert("Sucesso", "E-mail enviado!", "OK");
        });
    }
}
