using System.Threading.Tasks;
using Telemedicina.Domain.Entities;

namespace Telemedicina.Interfaces;

public interface IPatientRepository : IGenericRepository<Patient>
{
    Task<Patient?> GetByCpfAsync(string cpf);
}
