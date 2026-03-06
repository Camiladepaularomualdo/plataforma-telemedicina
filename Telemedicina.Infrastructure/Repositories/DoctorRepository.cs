using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Telemedicina.Domain.Entities;
using Telemedicina.Infrastructure.Data;
using Telemedicina.Interfaces;

namespace Telemedicina.Infrastructure.Repositories;

public class DoctorRepository : GenericRepository<Doctor>, IDoctorRepository
{
    public DoctorRepository(AppDbContext context) : base(context) { }

    public async Task<Doctor?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(d => d.Email == email);
    }
}
