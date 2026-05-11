using System.Text.Json.Serialization;

namespace ClinifyMobile.Models;

public class Appointment
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("doctorId")]
    public int DoctorId { get; set; }

    [JsonPropertyName("patientId")]
    public int PatientId { get; set; }

    [JsonPropertyName("patient")]
    public Patient? Patient { get; set; }

    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    /// <summary>
    /// API returns time as "HH:mm:ss" string (TimeSpan serialized).
    /// </summary>
    [JsonPropertyName("time")]
    public string Time { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public AppointmentStatus Status { get; set; }

    [JsonPropertyName("meetingUrl")]
    public string? MeetingUrl { get; set; }

    // ─── Computed helpers ───────────────────────────────────────────────────

    public string PatientName => Patient?.Name ?? "Paciente";

    public string FormattedDate => Date.ToString("dd/MM/yyyy");

    public string FormattedTime
    {
        get
        {
            if (string.IsNullOrEmpty(Time)) return "";
            var parts = Time.Split(':');
            return parts.Length >= 2 ? $"{parts[0]}:{parts[1]}" : Time;
        }
    }

    public string StatusLabel => Status switch
    {
        AppointmentStatus.Agendado  => "Agendado",
        AppointmentStatus.EmEspera  => "Em Espera",
        AppointmentStatus.Atendido  => "Atendido",
        AppointmentStatus.Faltou    => "Faltou",
        AppointmentStatus.Cancelado => "Cancelado",
        _ => "Desconhecido"
    };
}
