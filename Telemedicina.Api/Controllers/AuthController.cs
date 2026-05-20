using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Telemedicina.Domain.Entities;
using Telemedicina.Interfaces;

namespace Telemedicina.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IDoctorService _doctorService;
    private readonly IPatientService _patientService;

    public AuthController(IDoctorService doctorService, IPatientService patientService)
    {
        _doctorService = doctorService;
        _patientService = patientService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var doctor = await _doctorService.AuthenticateAsync(dto.Email, dto.Password);
        if (doctor == null) return Unauthorized("Invalid email or password");
        if (doctor.IsDeleted) return Unauthorized("Conta inativa. Entre em contato com o suporte da Clinfy.");
        
        return Ok(doctor);
    }

    [HttpPost("patient/login")]
    public async Task<IActionResult> PatientLogin([FromBody] LoginDto dto)
    {
        var patient = await _patientService.AuthenticateAsync(dto.Email, dto.Password);
        if (patient == null) return Unauthorized("Invalid email or password");
        
        return Ok(patient);
    }

    [HttpPost("patient/first-access")]
    public async Task<IActionResult> PatientFirstAccess([FromBody] FirstAccessDto dto)
    {
        var success = await _patientService.GenerateFirstAccessPasswordAsync(dto.Email);
        if (!success) return NotFound("Patient not found with this email");

        return Ok(new { message = "Seu primeiro acesso foi gerado com sucesso. Verifique seu e-mail." });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] Doctor doctor)
    {
        try
        {
            var created = await _doctorService.RegisterAsync(doctor);
            return Ok(created);
        }
        catch (System.Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

public class LoginDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class FirstAccessDto
{
    public string Email { get; set; } = string.Empty;
}
