using System;

namespace Telemedicina.Domain.Entities;

public class Doctor
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    
    // Email Integration Config
    public string? GmailAddress { get; set; }
    public string? GmailAppPassword { get; set; }

    // User Credits system
    public int Credits { get; set; } = 100;
}
