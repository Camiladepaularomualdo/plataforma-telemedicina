using System;

namespace Telemedicina.Domain.Entities;

public class CreditTransaction
{
    public int Id { get; set; }
    
    public int DoctorId { get; set; }
    public Doctor? Doctor { get; set; }

    public int? AppointmentId { get; set; }
    public Appointment? Appointment { get; set; }

    // How many credits used or added (-1 for usages, + for additions)
    public int Amount { get; set; }
    
    // e.g., "Geração de link de atendimento"
    public string Description { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
