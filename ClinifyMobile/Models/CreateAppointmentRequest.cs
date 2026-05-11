using System.Text.Json.Serialization;

namespace ClinifyMobile.Models;

/// <summary>
/// DTO enviado para POST /appointments.
/// A API espera um objeto Appointment completo com DateTime e TimeSpan.
/// Usamos um objeto anônimo no PostAsync para controlar a serialização.
/// </summary>
public class CreateAppointmentRequest
{
    [JsonPropertyName("doctorId")]
    public int DoctorId { get; set; }

    [JsonPropertyName("patientId")]
    public int PatientId { get; set; }

    /// <summary>Data como DateTime (ex: 2026-05-11T00:00:00)</summary>
    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    /// <summary>Horário como string "HH:mm:ss" — a API aceita TimeSpan serializado</summary>
    [JsonPropertyName("time")]
    public string Time { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public int Status { get; set; } = 0; // 0 = Agendado
}
