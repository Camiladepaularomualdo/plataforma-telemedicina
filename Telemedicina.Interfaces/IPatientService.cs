using System.Collections.Generic;
using System.Threading.Tasks;
using Telemedicina.Domain.Entities;

namespace Telemedicina.Interfaces;

public interface IPatientService
{
    Task<Patient> RegisterAsync(Patient patient);
    Task<Patient?> GetByIdAsync(int id);
    Task<IEnumerable<Patient>> GetAllAsync();
    Task<IEnumerable<Patient>> GetByDoctorIdAsync(int doctorId);
}
