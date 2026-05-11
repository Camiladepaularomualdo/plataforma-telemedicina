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
    /// Encerra a sessão e limpa o SecureStorage.
    /// </summary>
    public void Logout()
    {
        _session.Clear();
    }

    public bool IsLoggedIn => _session.IsLoggedIn;
}
