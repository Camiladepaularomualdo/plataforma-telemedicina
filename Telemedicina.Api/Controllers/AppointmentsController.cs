using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Telemedicina.Domain.Entities;
using Telemedicina.Interfaces;

namespace Telemedicina.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentService _appointmentService;

    public AppointmentsController(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    [HttpGet("doctor/{doctorId}/year/{year}/month/{month}")]
    public async Task<IActionResult> GetByDoctorAndMonth(int doctorId, int year, int month)
    {
        var appointments = await _appointmentService.GetDoctorAppointmentsByMonthAsync(doctorId, year, month);
        return Ok(appointments);
    }

    [HttpGet("doctor/{doctorId}/all")]
    public async Task<IActionResult> GetAllByDoctor(int doctorId)
    {
        var appointments = await _appointmentService.GetDoctorAppointmentsAsync(doctorId);
        return Ok(appointments);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Appointment appointment)
    {
        try
        {
            var created = await _appointmentService.CreateAppointmentAsync(appointment);
            return Ok(created);
        }
        catch (System.Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPatch("{id}/status/{status}")]
    public async Task<IActionResult> UpdateStatus(int id, int status)
    {
        var success = await _appointmentService.UpdateStatusAsync(id, (Telemedicina.Domain.Enums.AppointmentStatus)status);
        if (!success) return NotFound();
        return Ok();
    }

    [HttpPost("{id}/generate-meeting")]
    public async Task<IActionResult> GenerateMeetingUrl(int id, [FromServices] Microsoft.Extensions.Configuration.IConfiguration config)
    {
        var apiKey = config["Whereby:ApiKey"];
        if (string.IsNullOrEmpty(apiKey) || apiKey.Contains("YOUR_WHEREBY_API_KEY"))
            return BadRequest("Whereby API Key not configured properly.");

        try
        {
            var url = await _appointmentService.GenerateMeetingUrlAsync(id, apiKey);
            if (url == null) return NotFound("Appointment not found or failed to generate URL from Whereby.");
            return Ok(new { meetingUrl = url });
        }
        catch (System.Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPost("{id}/send-email")]
    public async Task<IActionResult> SendMeetingEmail(int id, [FromServices] IEmailService emailService, [FromServices] IAppointmentRepository appointmentRepository, [FromServices] IDoctorService doctorService)
    {
        var appointment = await appointmentRepository.GetAppointmentWithPatientAsync(id);
        if (appointment == null) return NotFound("Appointment not found.");
        if (string.IsNullOrEmpty(appointment.MeetingUrl)) return BadRequest("Esta consulta ainda não tem uma URL gerada.");
        if (appointment.Patient == null || string.IsNullOrEmpty(appointment.Patient.Email)) return BadRequest("Paciente não possui email cadastrado.");
        if (appointment.DoctorId <= 0) return BadRequest("Médico inválido para esta consulta.");

        var doctor = await doctorService.GetGmailConfigAsync(appointment.DoctorId);
        if (doctor == null || string.IsNullOrEmpty(doctor.GmailAddress) || string.IsNullOrEmpty(doctor.GmailAppPassword))
        {
            return BadRequest("Doctor Gmail configuration is missing.");
        }

        try
        {
            await emailService.SendMeetingUrlEmailAsync(appointment.Patient.Email, doctor.Name, appointment.MeetingUrl, doctor.GmailAddress, doctor.GmailAppPassword);
            return Ok();
        }
        catch (System.Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao enviar email: " + ex.Message });
        }
    }
}
