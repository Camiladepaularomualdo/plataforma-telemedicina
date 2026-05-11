using System.Windows.Input;
using ClinifyMobile.Models;
using ClinifyMobile.Services;
using ClinifyMobile.ViewModels.Base;

namespace ClinifyMobile.ViewModels;

/// <summary>
/// ViewModel para cadastro de novo paciente.
/// Equivale ao PatientFormComponent do Angular.
/// </summary>
public class AddPatientViewModel : BaseViewModel
{
    private readonly PatientService _patientService;

    public AddPatientViewModel(PatientService patientService)
    {
        _patientService = patientService;
        Title = "Novo Paciente";

        SaveCommand   = new Command(async () => await SaveAsync(), () => !IsBusy);
        CancelCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
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

    private string _phone = string.Empty;
    public string Phone
    {
        get => _phone;
        set => SetProperty(ref _phone, value);
    }

    private string _cpf = string.Empty;
    public string Cpf
    {
        get => _cpf;
        set => SetProperty(ref _cpf, value);
    }

    private DateTime _birthDate = DateTime.Today.AddYears(-25);
    public DateTime BirthDate
    {
        get => _birthDate;
        set => SetProperty(ref _birthDate, value);
    }

    // ─── Commands ─────────────────────────────────────────────────────────────

    public ICommand SaveCommand   { get; }
    public ICommand CancelCommand { get; }

    // ─── Logic ───────────────────────────────────────────────────────────────

    private async Task SaveAsync()
    {
        if (!Validate()) return;

        await ExecuteAsync(async () =>
        {
            var request = new CreatePatientRequest
            {
                Name      = Name.Trim(),
                Email     = Email.Trim(),
                Phone     = Phone.Trim(),
                Cpf       = Cpf.Trim(),
                BirthDate = BirthDate.ToString("yyyy-MM-dd")
            };

            await _patientService.CreateAsync(request);

            await Shell.Current.DisplayAlert("Sucesso", "Paciente cadastrado com sucesso!", "OK");
            await Shell.Current.GoToAsync("..");
        });
    }

    private bool Validate()
    {
        ClearError();

        if (string.IsNullOrWhiteSpace(Name))
        {
            ErrorMessage = "O nome é obrigatório.";
            return false;
        }
        if (string.IsNullOrWhiteSpace(Email) || !Email.Contains('@'))
        {
            ErrorMessage = "Informe um e-mail válido.";
            return false;
        }
        if (string.IsNullOrWhiteSpace(Phone))
        {
            ErrorMessage = "O telefone é obrigatório.";
            return false;
        }
        if (string.IsNullOrWhiteSpace(Cpf) || Cpf.Replace(".", "").Replace("-", "").Length != 11)
        {
            ErrorMessage = "Informe um CPF válido (11 dígitos).";
            return false;
        }

        return true;
    }
}
