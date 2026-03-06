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
}
