using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Telemedicina.Domain.Entities;
using Telemedicina.Interfaces;

namespace Telemedicina.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PatientsController : ControllerBase
{
    private readonly IPatientService _patientService;

    public PatientsController(IPatientService patientService)
    {
        _patientService = patientService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var patients = await _patientService.GetAllAsync();
        return Ok(patients);
    }

    [HttpGet("doctor/{doctorId}")]
    public async Task<IActionResult> GetByDoctorId(int doctorId)
    {
        var patients = await _patientService.GetByDoctorIdAsync(doctorId);
        return Ok(patients);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var patient = await _patientService.GetByIdAsync(id);
        if (patient == null) return NotFound();
        return Ok(patient);
    }

    [HttpPost]
    public async Task<IActionResult> Register([FromBody] Patient patient)
    {
        try
        {
            var created = await _patientService.RegisterAsync(patient);
            return Ok(created);
        }
        catch (System.Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
