using ClinifyMobile.Models;

namespace ClinifyMobile.Services;

/// <summary>
/// Gerencia operações de agendamentos consumindo a API existente.
/// </summary>
public class AppointmentService
{
    private readonly ApiService     _api;
    private readonly SessionService _session;

    public AppointmentService(ApiService api, SessionService session)
    {
        _api     = api;
        _session = session;
    }

    // ─── GET: Agendamentos do mês ─────────────────────────────────────────

    /// <summary>GET /appointments/doctor/{id}/year/{y}/month/{m}</summary>
    public async Task<List<Appointment>> GetByMonthAsync(int year, int month)
    {
        var doctorId = _session.DoctorId;
        var result = await _api.GetAsync<List<Appointment>>(
            $"appointments/doctor/{doctorId}/year/{year}/month/{month}");
        return result ?? [];
    }

    // ─── GET: Todos os agendamentos ───────────────────────────────────────

    /// <summary>GET /appointments/doctor/{id}/all</summary>
    public async Task<List<Appointment>> GetAllAsync()
    {
        var doctorId = _session.DoctorId;
        var result = await _api.GetAsync<List<Appointment>>(
            $"appointments/doctor/{doctorId}/all");
        return result ?? [];
    }

    // ─── POST: Criar agendamento ──────────────────────────────────────────

    /// <summary>POST /appointments — usando o request tipado</summary>
    public async Task<Appointment?> CreateAsync(CreateAppointmentRequest request)
    {
        request.DoctorId = _session.DoctorId;
        return await _api.PostAsync<Appointment>("appointments", request);
    }

    /// <summary>POST /appointments — usando payload anônimo (formato exato do Angular)</summary>
    public async Task<Appointment?> CreateRawAsync(object payload)
    {
        return await _api.PostAsync<Appointment>("appointments", payload);
    }

    // ─── PATCH: Alterar status ────────────────────────────────────────────

    /// <summary>PATCH /appointments/{id}/status/{status}</summary>
    public async Task UpdateStatusAsync(int appointmentId, AppointmentStatus newStatus)
    {
        await _api.PatchAsync($"appointments/{appointmentId}/status/{(int)newStatus}");
    }

    // ─── POST: Gerar sala de reunião ──────────────────────────────────────

    /// <summary>POST /appointments/{id}/generate-meeting</summary>
    public async Task<string?> GenerateMeetingAsync(int appointmentId)
    {
        var result = await _api.PostAsync<MeetingUrlResponse>(
            $"appointments/{appointmentId}/generate-meeting", new { });
        return result?.MeetingUrl;
    }

    // ─── POST: Enviar e-mail para paciente ────────────────────────────────

    /// <summary>POST /appointments/{id}/send-email</summary>
    public async Task SendEmailAsync(int appointmentId)
    {
        await _api.PostAsync($"appointments/{appointmentId}/send-email");
    }
}

// Classe auxiliar para desserializar a resposta do generate-meeting
file class MeetingUrlResponse
{
    public string? MeetingUrl { get; set; }
}
