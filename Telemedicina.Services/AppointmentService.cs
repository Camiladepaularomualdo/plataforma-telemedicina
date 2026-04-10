using System.Collections.Generic;
using System.Threading.Tasks;
using Telemedicina.Domain.Entities;
using Telemedicina.Interfaces;

namespace Telemedicina.Services;

public class AppointmentService : IAppointmentService
{
    private readonly IAppointmentRepository _repository;

    public AppointmentService(IAppointmentRepository repository)
    {
        _repository = repository;
    }

    public async Task<Appointment> CreateAppointmentAsync(Appointment appointment)
    {
        // Simple logic for booking
        appointment.Status = Domain.Enums.AppointmentStatus.Agendado;
        await _repository.AddAsync(appointment);
        await _repository.SaveChangesAsync();

        return appointment;
    }

    public async Task<IEnumerable<Appointment>> GetDoctorAppointmentsByMonthAsync(int doctorId, int year, int month)
    {
        return await _repository.GetDoctorAppointmentsByMonthAsync(doctorId, year, month);
    }

    public async Task<IEnumerable<Appointment>> GetDoctorAppointmentsAsync(int doctorId)
    {
        return await _repository.GetDoctorAppointmentsAsync(doctorId);
    }

    public async Task<bool> UpdateStatusAsync(int appointmentId, Telemedicina.Domain.Enums.AppointmentStatus status)
    {
        var appointment = await _repository.GetByIdAsync(appointmentId);
        if (appointment == null) return false;

        appointment.Status = status;
        _repository.Update(appointment);
        await _repository.SaveChangesAsync();
        return true;
    }

    public async Task<string?> GenerateMeetingUrlAsync(int appointmentId, string apiKey)
    {
        var appointment = await _repository.GetByIdAsync(appointmentId);
        if (appointment == null) return null;

        if (!string.IsNullOrEmpty(appointment.MeetingUrl)) return appointment.MeetingUrl;

        using var client = new System.Net.Http.HttpClient();
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

        // Whereby expects endDate. We add 1 hour from appointment time as a default.
        var endDate = appointment.Date.Date.Add(appointment.Time).AddHours(4).ToString("yyyy-MM-ddTHH:mm:ssZ");

        var requestBody = new { endDate = endDate, fields = new[] { "hostRoomUrl" } };
        var content = new System.Net.Http.StringContent(System.Text.Json.JsonSerializer.Serialize(requestBody), System.Text.Encoding.UTF8, "application/json");

        var response = await client.PostAsync("https://api.whereby.dev/v1/meetings", content);
        if (response.IsSuccessStatusCode)
        {
            var responseString = await response.Content.ReadAsStringAsync();
            using var document = System.Text.Json.JsonDocument.Parse(responseString);
            var roomUrl = document.RootElement.GetProperty("roomUrl").GetString();

            appointment.MeetingUrl = roomUrl;
            _repository.Update(appointment);
            await _repository.SaveChangesAsync();

            return roomUrl;
        }

        return null;
    }
}
