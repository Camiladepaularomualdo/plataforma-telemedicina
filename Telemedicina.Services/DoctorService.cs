using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Telemedicina.Domain.Entities;
using Telemedicina.Interfaces;

namespace Telemedicina.Services;

public class DoctorService : IDoctorService
{
    private readonly IDoctorRepository _repository;

    public DoctorService(IDoctorRepository repository)
    {
        _repository = repository;
    }

    public async Task<Doctor?> AuthenticateAsync(string email, string password)
    {
        var doctor = await _repository.GetByEmailAsync(email);
        if (doctor == null) return null;

        var hash = ComputeHash(password);
        if (doctor.PasswordHash == hash)
        {
            return doctor;
        }
        return null;
    }

    public async Task<Doctor> RegisterAsync(Doctor doctor)
    {
        var existing = await _repository.GetByEmailAsync(doctor.Email);
        if (existing != null) throw new Exception("Email already registered");

        doctor.PasswordHash = ComputeHash(doctor.PasswordHash);
        await _repository.AddAsync(doctor);
        await _repository.SaveChangesAsync();

        return doctor;
    }

    private string ComputeHash(string input)
    {
        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(input);
        var hash = sha.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
