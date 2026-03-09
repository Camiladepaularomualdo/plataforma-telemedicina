using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telemedicina.Domain.Entities;
using Telemedicina.Infrastructure.Data;
using Telemedicina.Interfaces;

namespace Telemedicina.Infrastructure.Repositories;

public class AppointmentRepository : GenericRepository<Appointment>, IAppointmentRepository
{
    public AppointmentRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Appointment>> GetDoctorAppointmentsByMonthAsync(int doctorId, int year, int month)
    {
        return await _dbSet
            .Include(a => a.Patient)
            .Where(a => a.DoctorId == doctorId && a.Date.Year == year && a.Date.Month == month)
            .OrderBy(a => a.Date)
            .ThenBy(a => a.Time)
            .ToListAsync();
    }

    public async Task<IEnumerable<Appointment>> GetDoctorAppointmentsAsync(int doctorId)
    {
        return await _dbSet
            .Include(a => a.Patient)
            .Where(a => a.DoctorId == doctorId)
            .OrderByDescending(a => a.Date)
            .ThenBy(a => a.Time)
            .ToListAsync();
    }

    public async Task<Appointment?> GetAppointmentWithPatientAsync(int id)
    {
        return await _dbSet
            .Include(a => a.Patient)
            .FirstOrDefaultAsync(a => a.Id == id);
    }
}
