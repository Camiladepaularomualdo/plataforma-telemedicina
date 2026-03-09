using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Telemedicina.Interfaces;

namespace Telemedicina.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DoctorsController : ControllerBase
{
    private readonly IDoctorService _doctorService;

    public DoctorsController(IDoctorService doctorService)
    {
        _doctorService = doctorService;
    }

    [HttpGet("{id}/gmail-config")]
    public async Task<IActionResult> GetGmailConfig(int id)
    {
        var doctor = await _doctorService.GetGmailConfigAsync(id);
        if (doctor == null) return NotFound();

        return Ok(new
        {
            hasConfig = !string.IsNullOrEmpty(doctor.GmailAddress) && !string.IsNullOrEmpty(doctor.GmailAppPassword),
            gmailAddress = doctor.GmailAddress
        });
    }

    [HttpPost("{id}/gmail-config")]
    public async Task<IActionResult> SaveGmailConfig(int id, [FromBody] GmailConfigRequest request)
    {
        if (string.IsNullOrEmpty(request.GmailAddress) || string.IsNullOrEmpty(request.GmailAppPassword))
            return BadRequest("Gmail address and app password are required.");

        var success = await _doctorService.SaveGmailConfigAsync(id, request.GmailAddress, request.GmailAppPassword);
        if (!success) return NotFound();

        return Ok();
    }
}

public class GmailConfigRequest
{
    public string GmailAddress { get; set; } = string.Empty;
    public string GmailAppPassword { get; set; } = string.Empty;
}
