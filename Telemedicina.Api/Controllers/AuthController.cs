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

    public AuthController(IDoctorService doctorService)
    {
        _doctorService = doctorService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var doctor = await _doctorService.AuthenticateAsync(dto.Email, dto.Password);
        if (doctor == null) return Unauthorized("Invalid email or password");
        if (doctor.IsDeleted) return Unauthorized("Conta inativa. Entre em contato com o suporte da Clinfy.");
        
        return Ok(doctor);
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
