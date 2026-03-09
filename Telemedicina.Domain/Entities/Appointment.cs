using System;
using Telemedicina.Domain.Enums;

namespace Telemedicina.Domain.Entities;

public class Appointment
{
    public int Id { get; set; }
    public int DoctorId { get; set; }
    public Doctor? Doctor { get; set; }
    public int PatientId { get; set; }
    public Patient? Patient { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan Time { get; set; }
    public AppointmentStatus Status { get; set; }
    public string? MeetingUrl { get; set; }
}
