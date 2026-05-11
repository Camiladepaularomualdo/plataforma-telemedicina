using ClinifyMobile.Models;

namespace ClinifyMobile.Services;

public class DoctorService
{
    private readonly ApiService _api;
    private readonly SessionService _session;

    public DoctorService(ApiService api, SessionService session)
    {
        _api = api;
        _session = session;
    }

    public async Task<GmailConfigResponse?> GetGmailConfigAsync()
    {
        var doctorId = _session.DoctorId;
        return await _api.GetAsync<GmailConfigResponse>($"doctors/{doctorId}/gmail-config");
    }

    public async Task SaveGmailConfigAsync(string email, string appPassword)
    {
        var doctorId = _session.DoctorId;
        var request = new { GmailAddress = email, GmailAppPassword = appPassword };
        await _api.PostAsync($"doctors/{doctorId}/gmail-config", request);
    }
}

public class GmailConfigResponse
{
    public bool HasConfig { get; set; }
    public string? GmailAddress { get; set; }
}
