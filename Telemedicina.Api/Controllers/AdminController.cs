using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Telemedicina.Infrastructure.Data;

namespace Telemedicina.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly AppDbContext _context;

    public AdminController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("doctors-report")]
    public async Task<IActionResult> GetDoctorsReport([FromQuery] int requestingDoctorId)
    {
        // Validate that the requesting doctor has admin privileges
        var requestingDoctor = await _context.Doctors.FirstOrDefaultAsync(d => d.Id == requestingDoctorId);
        if (requestingDoctor == null)
            return NotFound("Doctor not found.");

        if (requestingDoctor.Rule != "adm" && requestingDoctor.Rule != "all")
            return StatusCode(403, "Acesso negado. Apenas administradores podem acessar este relatório.");

        var report = await _context.Doctors
            .Select(d => new
            {
                d.Id,
                d.Name,
                d.Email,
                d.Credits,
                d.PlanCredits,
                d.CreatedAt,
                d.Rule,
                d.IsDeleted,
                TotalCreditsUsed = _context.CreditTransactions
                    .Where(t => t.DoctorId == d.Id && t.Amount < 0)
                    .Sum(t => t.Amount) * -1,
                TotalPatients = _context.Patients
                    .Count(p => p.DoctorId == d.Id),
                TotalAppointments = _context.Appointments
                    .Count(a => a.DoctorId == d.Id)
            })
            .OrderBy(d => d.Name)
            .ToListAsync();

        return Ok(report);
    }

    [HttpPatch("doctor/{doctorId}/update-plan")]
    public async Task<IActionResult> UpdateDoctorPlan(int doctorId, [FromQuery] int requestingDoctorId, [FromBody] UpdatePlanRequest request)
    {
        // Validate admin privileges
        var requestingDoctor = await _context.Doctors.FirstOrDefaultAsync(d => d.Id == requestingDoctorId);
        if (requestingDoctor == null)
            return NotFound("Requesting doctor not found.");

        if (requestingDoctor.Rule != "adm" && requestingDoctor.Rule != "all")
            return StatusCode(403, "Acesso negado. Apenas administradores podem realizar esta ação.");

        // Find target doctor
        var targetDoctor = await _context.Doctors.FirstOrDefaultAsync(d => d.Id == doctorId);
        if (targetDoctor == null)
            return NotFound("Doctor not found.");

        if (request.NewPlanCredits < 0)
            return BadRequest("O plano de créditos não pode ser negativo.");

        targetDoctor.PlanCredits = request.NewPlanCredits;
        targetDoctor.Credits = request.NewPlanCredits;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Plano atualizado com sucesso.", planCredits = targetDoctor.PlanCredits, credits = targetDoctor.Credits });
    }
    [HttpPatch("doctor/{doctorId}/toggle-status")]
    public async Task<IActionResult> ToggleDoctorStatus(int doctorId, [FromQuery] int requestingDoctorId, [FromBody] ToggleStatusRequest request)
    {
        // Validate admin privileges
        var requestingDoctor = await _context.Doctors.FirstOrDefaultAsync(d => d.Id == requestingDoctorId);
        if (requestingDoctor == null)
            return NotFound("Requesting doctor not found.");

        if (requestingDoctor.Rule != "adm" && requestingDoctor.Rule != "all")
            return StatusCode(403, "Acesso negado. Apenas administradores podem realizar esta ação.");

        // Find target doctor
        var targetDoctor = await _context.Doctors.FirstOrDefaultAsync(d => d.Id == doctorId);
        if (targetDoctor == null)
            return NotFound("Doctor not found.");

        if (targetDoctor.Id == requestingDoctorId)
            return BadRequest("Você não pode alterar o próprio status.");

        targetDoctor.IsDeleted = request.IsDeleted;
        await _context.SaveChangesAsync();

        var message = request.IsDeleted ? "Médico excluído com sucesso." : "Médico ativado com sucesso.";
        return Ok(new { message, isDeleted = targetDoctor.IsDeleted });
    }
}

public class UpdatePlanRequest
{
    public int NewPlanCredits { get; set; }
}

public class ToggleStatusRequest
{
    public bool IsDeleted { get; set; }
}
