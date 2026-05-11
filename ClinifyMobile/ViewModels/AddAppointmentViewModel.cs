using System.Collections.ObjectModel;
using System.Windows.Input;
using ClinifyMobile.Models;
using ClinifyMobile.Services;
using ClinifyMobile.ViewModels.Base;

namespace ClinifyMobile.ViewModels;

public class AddAppointmentViewModel : BaseViewModel
{
    private readonly AppointmentService _appointmentService;
    private readonly PatientService     _patientService;
    private readonly SessionService     _session;

    public AddAppointmentViewModel(
        AppointmentService appointmentService,
        PatientService patientService,
        SessionService session)
    {
        _appointmentService = appointmentService;
        _patientService     = patientService;
        _session            = session;

        Title = "Novo Agendamento";

        LoadPatientsCommand = new Command(async () => await LoadPatientsAsync());
        SaveCommand         = new Command(async () => await SaveAsync());
        CancelCommand       = new Command(async () => await Shell.Current.GoToAsync(".."));
    }

    // ─── Properties ──────────────────────────────────────────────────────────

    private ObservableCollection<Patient> _patients = [];
    public ObservableCollection<Patient> Patients
    {
        get => _patients;
        set => SetProperty(ref _patients, value);
    }

    private Patient? _selectedPatient;
    public Patient? SelectedPatient
    {
        get => _selectedPatient;
        set => SetProperty(ref _selectedPatient, value);
    }

    private DateTime _selectedDate = DateTime.Today;
    public DateTime SelectedDate
    {
        get => _selectedDate;
        set => SetProperty(ref _selectedDate, value);
    }

    private TimeSpan _selectedTime = new(9, 0, 0);
    public TimeSpan SelectedTime
    {
        get => _selectedTime;
        set => SetProperty(ref _selectedTime, value);
    }

    public ICommand LoadPatientsCommand { get; }
    public ICommand SaveCommand         { get; }
    public ICommand CancelCommand       { get; }

    // ─── Load ────────────────────────────────────────────────────────────────

    public async Task LoadPatientsAsync()
    {
        await ExecuteAsync(async () =>
        {
            var list = await _patientService.GetByDoctorAsync();
            Patients = new ObservableCollection<Patient>(list);
        });
    }

    // ─── Save ────────────────────────────────────────────────────────────────

    private async Task SaveAsync()
    {
        // Validação local
        if (SelectedPatient is null)
        {
            await Shell.Current.DisplayAlert("Atenção", "Selecione um paciente.", "OK");
            return;
        }

        if (IsBusy) return;
        IsBusy = true;

        try
        {
            var doctorId = _session.DoctorId;

            // ── monta payload idêntico ao Angular ──────────────────────────
            // Angular envia: { doctorId, patientId, date:"yyyy-MM-dd", time:"HH:mm:ss" }
            var payload = new
            {
                doctorId  = doctorId,
                patientId = SelectedPatient.Id,
                date      = SelectedDate.ToString("yyyy-MM-ddT00:00:00"),
                time      = $"{SelectedTime.Hours:D2}:{SelectedTime.Minutes:D2}:00",
                status    = 0
            };

            var created = await _appointmentService.CreateRawAsync(payload);

            // ── SUCESSO: mostra alert ANTES de navegar ─────────────────────
            await Shell.Current.DisplayAlert(
                "✅ Agendamento Criado!",
                $"Paciente : {SelectedPatient.Name}\n" +
                $"Data     : {SelectedDate:dd/MM/yyyy}\n" +
                $"Horário  : {SelectedTime.Hours:D2}:{SelectedTime.Minutes:D2}\n" +
                $"ID gerado: #{created?.Id}",
                "OK");

            // Navega DEPOIS do alert
            await Shell.Current.GoToAsync("..");
        }
        catch (ApiException ex)
        {
            // Erro da API — sempre visível como popup
            await Shell.Current.DisplayAlert(
                "❌ Erro ao Agendar",
                ex.Message,
                "OK");
        }
        catch (Exception ex)
        {
            // Erro inesperado — mostra tipo + mensagem para diagnóstico
            await Shell.Current.DisplayAlert(
                "❌ Erro Inesperado",
                $"Tipo: {ex.GetType().Name}\n{ex.Message}",
                "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
