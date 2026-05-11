namespace ClinifyMobile.Services;

/// <summary>
/// Gerencia a sessão autenticada do médico.
///
/// IMPORTANTE: Usa cache em memória para evitar deadlock no Android.
/// SecureStorage.GetAsync() NÃO pode ser chamado com .GetAwaiter().GetResult()
/// na thread principal — causa falha silenciosa retornando null.
///
/// Fluxo:
///   - Login → SaveSessionAsync → grava em memória + SecureStorage
///   - Startup → LoadFromStorageAsync → restaura memória do SecureStorage
///   - Leitura → sempre da memória (síncrona, segura)
/// </summary>
public class SessionService
{
    private const string KeyDoctorId   = "clinfy_doctorId";
    private const string KeyDoctorName = "clinfy_doctorName";
    private const string KeyDoctorRule = "clinfy_doctorRule";
    private const string KeyToken      = "clinfy_authToken";

    // ─── Cache em memória (acesso síncrono, sem risco de deadlock) ────────────

    private int     _doctorId   = 0;
    private string  _doctorName = string.Empty;
    private string  _doctorRule = "usr";
    private string? _token      = null;

    // ─── Properties (leitura síncrona da memória) ────────────────────────────

    public int     DoctorId   => _doctorId;
    public string  DoctorName => _doctorName;
    public string  Rule       => _doctorRule;
    public string? Token      => _token;

    public bool IsLoggedIn => _doctorId > 0;

    // ─── RBAC helpers ────────────────────────────────────────────────────────

    public bool CanAccessAttendance => Rule is "usr" or "all";
    public bool CanAccessAdmin      => Rule is "adm" or "all";

    // ─── Salva sessão após login ─────────────────────────────────────────────

    public async Task SaveSessionAsync(int doctorId, string name, string rule, string? token = null)
    {
        // 1. Atualiza memória imediatamente (sem await, seguro e síncrono)
        _doctorId   = doctorId;
        _doctorName = name;
        _doctorRule = rule;
        _token      = token;

        // 2. Persiste no SecureStorage (para sobreviver ao restart do app)
        await SecureStorage.SetAsync(KeyDoctorId,   doctorId.ToString());
        await SecureStorage.SetAsync(KeyDoctorName, name);
        await SecureStorage.SetAsync(KeyDoctorRule, rule);
        if (!string.IsNullOrEmpty(token))
            await SecureStorage.SetAsync(KeyToken, token);
    }

    // ─── Restaura sessão do SecureStorage ao iniciar o app ──────────────────

    /// <summary>
    /// Deve ser chamado na inicialização do app (App.xaml.cs ou LoginPage).
    /// Lê o SecureStorage de forma async segura e popula o cache em memória.
    /// </summary>
    public async Task LoadFromStorageAsync()
    {
        if (_doctorId > 0) return; // Já está em memória

        var idStr = await SecureStorage.GetAsync(KeyDoctorId);
        if (int.TryParse(idStr, out var id) && id > 0)
        {
            _doctorId   = id;
            _doctorName = await SecureStorage.GetAsync(KeyDoctorName) ?? string.Empty;
            _doctorRule = await SecureStorage.GetAsync(KeyDoctorRule) ?? "usr";
            _token      = await SecureStorage.GetAsync(KeyToken);
        }
    }

    // ─── Logout ──────────────────────────────────────────────────────────────

    public void Clear()
    {
        _doctorId   = 0;
        _doctorName = string.Empty;
        _doctorRule = "usr";
        _token      = null;

        SecureStorage.Remove(KeyDoctorId);
        SecureStorage.Remove(KeyDoctorName);
        SecureStorage.Remove(KeyDoctorRule);
        SecureStorage.Remove(KeyToken);
    }
}
