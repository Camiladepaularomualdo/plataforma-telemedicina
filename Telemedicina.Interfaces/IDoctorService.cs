using System.Collections.Generic;
using System.Threading.Tasks;
using Telemedicina.Domain.Entities;

namespace Telemedicina.Interfaces;

public interface IDoctorService
{
    Task<Doctor?> AuthenticateAsync(string email, string password);
    Task<Doctor> RegisterAsync(Doctor doctor);
}
