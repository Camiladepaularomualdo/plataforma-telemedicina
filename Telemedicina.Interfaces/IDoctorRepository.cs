using System.Threading.Tasks;
using Telemedicina.Domain.Entities;

namespace Telemedicina.Interfaces;

public interface IDoctorRepository : IGenericRepository<Doctor>
{
    Task<Doctor?> GetByEmailAsync(string email);
}
