using System.Collections.Generic;
using System.Threading.Tasks;
using Telemedicina.Domain.Entities;

namespace Telemedicina.Interfaces;

public interface IAppointmentRepository : IGenericRepository<Appointment>
{
    Task<IEnumerable<Appointment>> GetDoctorAppointmentsByMonthAsync(int doctorId, int year, int month);
    Task<IEnumerable<Appointment>> GetDoctorAppointmentsAsync(int doctorId);
}
