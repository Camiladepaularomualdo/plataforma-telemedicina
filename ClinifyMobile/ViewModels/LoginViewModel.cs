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

    // ─── Commands ─────────────────────────────────────────────────────────────

    public ICommand LoginCommand { get; }
    public ICommand GoToRegisterCommand { get; }

    // ─── Logic ───────────────────────────────────────────────────────────────

    private async Task DoLoginAsync()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Preencha o e-mail e a senha.";
            return;
        }

        await ExecuteAsync(async () =>
        {
            var doctor = await _authService.LoginAsync(Email.Trim(), Password);

            // Navega para a página correta conforme o role
            var route = _session.CanAccessAttendance ? "//AgendaPage" : "//AgendaPage";
            await Shell.Current.GoToAsync(route);
        });
    }

    /// <summary>Verifica se já há sessão ativa ao abrir o app.</summary>
    public async Task CheckExistingSessionAsync()
    {
        if (_session.IsLoggedIn)
            await Shell.Current.GoToAsync("//AgendaPage");
    }

    private async Task GoToRegisterAsync()
    {
        await Shell.Current.GoToAsync("//RegisterPage");
    }
}
