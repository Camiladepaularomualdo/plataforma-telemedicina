using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Telemedicina.Infrastructure.Data;

namespace Telemedicina.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CreditsController : ControllerBase
{
    private readonly AppDbContext _context;

    public CreditsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("doctor/{doctorId}/balance")]
    public async Task<IActionResult> GetBalance(int doctorId)
    {
        var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.Id == doctorId);
        if (doctor == null) return NotFound("Doctor not found.");

        return Ok(new { credits = doctor.Credits });
    }

    [HttpGet("doctor/{doctorId}/statement")]
    public async Task<IActionResult> GetStatement(int doctorId)
    {
        var transactions = await _context.CreditTransactions
            .Include(t => t.Appointment)
                .ThenInclude(a => a.Patient)
            .Where(t => t.DoctorId == doctorId)
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new {
                t.Id,
                t.Amount,
                t.Description,
                t.CreatedAt,
                PatientName = t.Appointment != null && t.Appointment.Patient != null ? t.Appointment.Patient.Name : "N/A",
                AppointmentDate = t.Appointment != null ? t.Appointment.Date : (System.DateTime?)null,
                AppointmentTime = t.Appointment != null ? t.Appointment.Time : (System.TimeSpan?)null
            })
            .ToListAsync();

        return Ok(transactions);
    }
}
