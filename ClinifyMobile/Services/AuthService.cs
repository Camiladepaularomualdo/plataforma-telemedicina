using ClinifyMobile.Models;

namespace ClinifyMobile.Services;

/// <summary>
/// Gerencia autenticação: login, logout e verificação de sessão.
/// </summary>
public class AuthService
{
    private readonly ApiService     _api;
    private readonly SessionService _session;

    public AuthService(ApiService api, SessionService session)
    {
        _api     = api;
        _session = session;
    }

    /// <summary>
    /// Autentica o médico. Persiste a sessão em SecureStorage.
    /// Retorna o Doctor ou lança ApiException com mensagem amigável.
    /// </summary>
    public async Task<Doctor> LoginAsync(string email, string password)
    {
        var request = new LoginRequest { Email = email, Password = password };
        var doctor  = await _api.PostAsync<Doctor>("auth/login", request)
                      ?? throw new ApiException("Resposta inválida do servidor.");

        // Persiste sessão (mesma lógica do Angular com localStorage)
        await _session.SaveSessionAsync(doctor.Id, doctor.Name, doctor.Rule);

        return doctor;
    }

    /// <summary>
    /// Cadastra um novo médico e já realiza o login automaticamente.
    /// </summary>
    public async Task<Doctor> RegisterAsync(Doctor newDoctor)
    {
        var doctor = await _api.PostAsync<Doctor>("auth/register", newDoctor)
                      ?? throw new ApiException("Resposta inválida do servidor ao cadastrar.");

        // Realiza o "login" na sessão após o cadastro
        await _session.SaveSessionAsync(doctor.Id, doctor.Name, doctor.Rule);

        return doctor;
    }

    /// <summary>
    /// Encerra a sessão e limpa o SecureStorage.
    /// </summary>
    public void Logout()
    {
        _session.Clear();
    }

    public async Task<Patient> PatientLoginAsync(string email, string password)
    {
        var request = new LoginRequest { Email = email, Password = password };
        var patient = await _api.PostAsync<Patient>("auth/patient/login", request)
                      ?? throw new ApiException("Resposta inválida do servidor.");

        await _session.SavePatientSessionAsync(patient.Id, patient.Name);

        return patient;
    }

    public async Task<string> PatientFirstAccessAsync(string email)
    {
        var request = new { Email = email };
        var response = await _api.PostAsync<System.Collections.Generic.Dictionary<string, string>>("auth/patient/first-access", request)
                       ?? throw new ApiException("Resposta inválida do servidor.");

        return response.TryGetValue("message", out var msg) ? msg : "Senha solicitada com sucesso.";
    }

    public bool IsLoggedIn => _session.HasAnySession;
}
