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
    private const string KeyPatientId  = "clinfy_patientId";
    private const string KeyPatientName= "clinfy_patientName";
    private const string KeyToken      = "clinfy_authToken";

    // ─── Cache em memória (acesso síncrono, sem risco de deadlock) ────────────

    private int     _doctorId   = 0;
    private string  _doctorName = string.Empty;
    private string  _doctorRule = "usr";
    
    private int     _patientId  = 0;
    private string  _patientName= string.Empty;

    private string? _token      = null;

    // ─── Properties (leitura síncrona da memória) ────────────────────────────

    public int     DoctorId   => _doctorId;
    public string  DoctorName => _doctorName;
    public string  Rule       => _doctorRule;
    
    public int     PatientId  => _patientId;
    public string  PatientName=> _patientName;

    public string? Token      => _token;

    public bool IsLoggedIn => _doctorId > 0;
    public bool IsPatientLoggedIn => _patientId > 0;
    public bool HasAnySession => IsLoggedIn || IsPatientLoggedIn;

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

    public async Task SavePatientSessionAsync(int patientId, string name, string? token = null)
    {
        _patientId   = patientId;
        _patientName = name;
        _token       = token;

        await SecureStorage.SetAsync(KeyPatientId,   patientId.ToString());
        await SecureStorage.SetAsync(KeyPatientName, name);
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
        if (HasAnySession) return; // Já está em memória

        var doctorIdStr = await SecureStorage.GetAsync(KeyDoctorId);
        if (int.TryParse(doctorIdStr, out var dId) && dId > 0)
        {
            _doctorId   = dId;
            _doctorName = await SecureStorage.GetAsync(KeyDoctorName) ?? string.Empty;
            _doctorRule = await SecureStorage.GetAsync(KeyDoctorRule) ?? "usr";
        }

        var patientIdStr = await SecureStorage.GetAsync(KeyPatientId);
        if (int.TryParse(patientIdStr, out var pId) && pId > 0)
        {
            _patientId   = pId;
            _patientName = await SecureStorage.GetAsync(KeyPatientName) ?? string.Empty;
        }

        _token = await SecureStorage.GetAsync(KeyToken);
    }

    // ─── Logout ──────────────────────────────────────────────────────────────

    public void Clear()
    {
        _doctorId   = 0;
        _doctorName = string.Empty;
        _doctorRule = "usr";

        _patientId  = 0;
        _patientName= string.Empty;

        _token      = null;

        SecureStorage.Remove(KeyDoctorId);
        SecureStorage.Remove(KeyDoctorName);
        SecureStorage.Remove(KeyDoctorRule);
        
        SecureStorage.Remove(KeyPatientId);
        SecureStorage.Remove(KeyPatientName);

        SecureStorage.Remove(KeyToken);
    }
}
