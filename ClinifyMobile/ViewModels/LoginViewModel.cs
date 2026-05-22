using System.Windows.Input;
using ClinifyMobile.Services;
using ClinifyMobile.ViewModels.Base;

namespace ClinifyMobile.ViewModels;

public class LoginViewModel : BaseViewModel
{
    private readonly AuthService    _authService;
    private readonly SessionService _session;

    public LoginViewModel(AuthService authService, SessionService session)
    {
        _authService = authService;
        _session     = session;
        Title        = "Clinfy";

        LoginCommand = new Command(async () => await DoLoginAsync(), () => !IsBusy);
        GoToRegisterCommand = new Command(async () => await GoToRegisterAsync());
        SetProfileCommand = new Command<string>(p => SetProfile(p));
        ToggleFirstAccessCommand = new Command(ToggleFirstAccess);
        FirstAccessCommand = new Command(async () => await DoFirstAccessAsync(), () => !IsBusy);
    }

    // ─── Properties ──────────────────────────────────────────────────────────

    private string _email = string.Empty;
    public string Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
    }

    private string _password = string.Empty;
    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    private string _profileType = "doctor";
    public string ProfileType
    {
        get => _profileType;
        set { SetProperty(ref _profileType, value); OnPropertyChanged(nameof(IsDoctorProfile)); OnPropertyChanged(nameof(IsPatientProfile)); }
    }
    public bool IsDoctorProfile => ProfileType == "doctor";
    public bool IsPatientProfile => ProfileType == "patient";

    private bool _isFirstAccess = false;
    public bool IsFirstAccess
    {
        get => _isFirstAccess;
        set => SetProperty(ref _isFirstAccess, value);
    }

    private string _patientEmail = string.Empty;
    public string PatientEmail
    {
        get => _patientEmail;
        set => SetProperty(ref _patientEmail, value);
    }

    private string _patientPassword = string.Empty;
    public string PatientPassword
    {
        get => _patientPassword;
        set => SetProperty(ref _patientPassword, value);
    }

    private string _successMessage = string.Empty;
    public string SuccessMessage
    {
        get => _successMessage;
        set { SetProperty(ref _successMessage, value); OnPropertyChanged(nameof(HasSuccessMessage)); }
    }
    public bool HasSuccessMessage => !string.IsNullOrEmpty(SuccessMessage);

    // ─── Commands ─────────────────────────────────────────────────────────────

    public ICommand LoginCommand { get; }
    public ICommand GoToRegisterCommand { get; }
    public ICommand SetProfileCommand { get; }
    public ICommand ToggleFirstAccessCommand { get; }
    public ICommand FirstAccessCommand { get; }

    // ─── Logic ───────────────────────────────────────────────────────────────

    private void SetProfile(string profile)
    {
        ProfileType = profile;
        ErrorMessage = string.Empty;
        SuccessMessage = string.Empty;
    }

    private void ToggleFirstAccess()
    {
        IsFirstAccess = !IsFirstAccess;
        ErrorMessage = string.Empty;
        SuccessMessage = string.Empty;
    }

    private async Task DoLoginAsync()
    {
        ErrorMessage = string.Empty;
        SuccessMessage = string.Empty;

        if (IsDoctorProfile)
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Preencha o e-mail e a senha.";
                return;
            }

            await ExecuteAsync(async () =>
            {
                var doctor = await _authService.LoginAsync(Email.Trim(), Password);
                var route = _session.CanAccessAttendance ? "//AgendaPage" : "//AgendaPage";
                await Shell.Current.GoToAsync(route);
            });
        }
        else
        {
            if (string.IsNullOrWhiteSpace(PatientEmail) || string.IsNullOrWhiteSpace(PatientPassword))
            {
                ErrorMessage = "Preencha o e-mail e a senha.";
                return;
            }

            await ExecuteAsync(async () =>
            {
                var patient = await _authService.PatientLoginAsync(PatientEmail.Trim(), PatientPassword);
                await Shell.Current.GoToAsync("//PatientAppointmentsPage");
            });
        }
    }

    private async Task DoFirstAccessAsync()
    {
        ErrorMessage = string.Empty;
        SuccessMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(PatientEmail))
        {
            ErrorMessage = "Preencha o e-mail.";
            return;
        }

        await ExecuteAsync(async () =>
        {
            var msg = await _authService.PatientFirstAccessAsync(PatientEmail.Trim());
            SuccessMessage = msg;
            IsFirstAccess = false;
            PatientPassword = string.Empty;
        });
    }

    /// <summary>Verifica se já há sessão ativa ao abrir o app.</summary>
    public async Task CheckExistingSessionAsync()
    {
        if (_session.IsLoggedIn)
            await Shell.Current.GoToAsync("//AgendaPage");
        else if (_session.IsPatientLoggedIn)
            await Shell.Current.GoToAsync("//PatientAppointmentsPage");
    }

    private async Task GoToRegisterAsync()
    {
        await Shell.Current.GoToAsync("//RegisterPage");
    }
}
