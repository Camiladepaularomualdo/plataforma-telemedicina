using ClinifyMobile.Models;

namespace ClinifyMobile.Services;

/// <summary>
/// Gerencia operações de pacientes consumindo a API existente.
/// </summary>
public class PatientService
{
    private readonly ApiService     _api;
    private readonly SessionService _session;

    public PatientService(ApiService api, SessionService session)
    {
        _api     = api;
        _session = session;
    }

    /// <summary>GET /patients/doctor/{doctorId}</summary>
    public async Task<List<Patient>> GetByDoctorAsync()
    {
        var doctorId = _session.DoctorId;
        var result = await _api.GetAsync<List<Patient>>($"patients/doctor/{doctorId}");
        return result ?? [];
    }

    /// <summary>POST /patients</summary>
    public async Task<Patient?> CreateAsync(CreatePatientRequest request)
    {
        request.DoctorId = _session.DoctorId;
        return await _api.PostAsync<Patient>("patients", request);
    }
}
