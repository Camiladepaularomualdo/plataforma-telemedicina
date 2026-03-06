using System.Collections.Generic;
using System.Threading.Tasks;
using Telemedicina.Domain.Entities;

namespace Telemedicina.Interfaces;

public interface IAppointmentService
{
    Task<Appointment> CreateAppointmentAsync(Appointment appointment);
    Task<IEnumerable<Appointment>> GetDoctorAppointmentsByMonthAsync(int doctorId, int year, int month);
    Task<IEnumerable<Appointment>> GetDoctorAppointmentsAsync(int doctorId);
    Task<bool> UpdateStatusAsync(int appointmentId, Telemedicina.Domain.Enums.AppointmentStatus status);
}
