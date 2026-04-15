using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telemedicina.Domain.Entities;
using Telemedicina.Infrastructure.Data;
using Telemedicina.Interfaces;

namespace Telemedicina.Infrastructure.Repositories;

public class PatientRepository : GenericRepository<Patient>, IPatientRepository
{
    public PatientRepository(AppDbContext context) : base(context) { }

    public async Task<Patient?> GetByCpfAsync(string cpf)
    {
        return await _dbSet.FirstOrDefaultAsync(p => p.Cpf == cpf);
    }

    public async Task<IEnumerable<Patient>> GetByDoctorIdAsync(int doctorId)
    {
        return await _dbSet.Where(p => p.DoctorId == doctorId).ToListAsync();
    }
}
