using System.Windows.Input;
using ClinifyMobile.Models;
using ClinifyMobile.Services;
using ClinifyMobile.ViewModels.Base;

namespace ClinifyMobile.ViewModels;

public class RegisterViewModel : BaseViewModel
{
    private readonly AuthService _authService;
    private readonly SessionService _session;

    public RegisterViewModel(AuthService authService, SessionService session)
    {
        _authService = authService;
        _session = session;
        Title = "Cadastro de Médico";

        RegisterCommand = new Command(async () => await DoRegisterAsync(), () => !IsBusy);
        GoToLoginCommand = new Command(async () => await GoToLoginAsync());
    }

    // ─── Properties ──────────────────────────────────────────────────────────

    private string _name = string.Empty;
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

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

    private string _cpf = string.Empty;
    public string Cpf
    {
        get => _cpf;
        set => SetProperty(ref _cpf, value);
    }

    private string _phone = string.Empty;
    public string Phone
    {
        get => _phone;
        set => SetProperty(ref _phone, value);
    }

    private DateTime _birthDate = DateTime.Today;
    public DateTime BirthDate
    {
        get => _birthDate;
        set => SetProperty(ref _birthDate, value);
    }

    // ─── Commands ─────────────────────────────────────────────────────────────

    public ICommand RegisterCommand { get; }
    public ICommand GoToLoginCommand { get; }

    // ─── Logic ───────────────────────────────────────────────────────────────

    private async Task DoRegisterAsync()
    {
        if (string.IsNullOrWhiteSpace(Name) || 
            string.IsNullOrWhiteSpace(Email) || 
            string.IsNullOrWhiteSpace(Password) ||
            string.IsNullOrWhiteSpace(Cpf))
        {
            ErrorMessage = "Preencha os campos obrigatórios (Nome, E-mail, Senha e CPF).";
            return;
        }

        if (Password.Length < 6)
        {
            ErrorMessage = "A senha deve ter no mínimo 6 caracteres.";
            return;
        }

        await ExecuteAsync(async () =>
        {
            var doctorToRegister = new Doctor
            {
                Name = Name.Trim(),
                Email = Email.Trim(),
                PasswordHash = Password, // Backend vai aplicar o Hash se necessário, na web envia Password como string, mas a entidade no C# mobile mapeia pra PasswordHash, vamos usar PasswordHash. Depende da estrutura, no web ele manda `password`.
                Cpf = Cpf.Trim(),
                Phone = string.IsNullOrWhiteSpace(Phone) ? string.Empty : Phone.Trim(),
                BirthDate = BirthDate,
                Rule = "usr"
            };

            var doctor = await _authService.RegisterAsync(doctorToRegister);

            // Redireciona para Agenda após cadastro (e auto-login)
            await Shell.Current.GoToAsync("//AgendaPage");
        });
    }

    private async Task GoToLoginAsync()
    {
        // Retorna para a página de Login
        await Shell.Current.GoToAsync("//LoginPage");
    }
}
