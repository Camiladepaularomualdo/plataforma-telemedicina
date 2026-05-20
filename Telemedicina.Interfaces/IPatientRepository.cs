using System.Collections.Generic;
using System.Threading.Tasks;
using Telemedicina.Domain.Entities;

namespace Telemedicina.Interfaces;

public interface IPatientRepository : IGenericRepository<Patient>
{
    Task<Patient?> GetByCpfAsync(string cpf);
    Task<Patient?> GetByEmailAsync(string email);
    Task<IEnumerable<Patient>> GetByDoctorIdAsync(int doctorId);
}
