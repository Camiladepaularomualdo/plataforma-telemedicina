namespace ClinifyMobile.Models;

public class CreatePatientRequest
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string BirthDate { get; set; } = string.Empty;  // "yyyy-MM-dd"
    public string Cpf { get; set; } = string.Empty;
    public int DoctorId { get; set; }
}
