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
    public int Credits { get; set; } = 15;
    
    // Credit Renewal System
    public int PlanCredits { get; set; } = 15;
    public DateTime? LastRenewalDate { get; set; }
    public DateTime? NextRenewalDate { get; set; }

    // Role-Based Access Control (adm, usr, all)
    public string Rule { get; set; } = "usr";

    // Registration timestamp
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Soft delete flag
    public bool IsDeleted { get; set; } = false;
}
