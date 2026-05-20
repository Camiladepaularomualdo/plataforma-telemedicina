using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telemedicina.Domain.Entities;
using Telemedicina.Interfaces;

namespace Telemedicina.Services;

public class PatientService : IPatientService
{
    private readonly IPatientRepository _repository;
    private readonly IEmailService _emailService;
    private readonly IDoctorRepository _doctorRepository;

    public PatientService(IPatientRepository repository, IEmailService emailService, IDoctorRepository doctorRepository)
    {
        _repository = repository;
        _emailService = emailService;
        _doctorRepository = doctorRepository;
    }

    public async Task<Patient> RegisterAsync(Patient patient)
    {
        var existing = await _repository.GetByCpfAsync(patient.Cpf);
        if (existing != null) throw new Exception("CPF already registered");

        await _repository.AddAsync(patient);
        await _repository.SaveChangesAsync();

        return patient;
    }

    public async Task<Patient?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Patient>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<IEnumerable<Patient>> GetByDoctorIdAsync(int doctorId)
    {
        return await _repository.GetByDoctorIdAsync(doctorId);
    }

    public async Task<Patient?> AuthenticateAsync(string email, string password)
    {
        var patient = await _repository.GetByEmailAsync(email);
        if (patient == null) return null;
        
        // Em um cenário real, validar hash de senha
        if (patient.PasswordHash != password) return null;

        return patient;
    }

    public async Task<bool> GenerateFirstAccessPasswordAsync(string email)
    {
        var patient = await _repository.GetByEmailAsync(email);
        if (patient == null) return false;

        var tempPassword = Guid.NewGuid().ToString().Substring(0, 8);
        patient.PasswordHash = tempPassword;
        _repository.Update(patient);
        await _repository.SaveChangesAsync();

        if (patient.DoctorId.HasValue)
        {
            var doctor = await _doctorRepository.GetByIdAsync(patient.DoctorId.Value);
            if (doctor != null && !string.IsNullOrEmpty(doctor.GmailAddress) && !string.IsNullOrEmpty(doctor.GmailAppPassword))
            {
                try
                {
                    await _emailService.SendTemporaryPasswordEmailAsync(patient.Email, doctor.Name, tempPassword, doctor.GmailAddress, doctor.GmailAppPassword);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao enviar e-mail de senha temporária: {ex.Message}");
                    // Continua mesmo se falhar o envio (para não quebrar a geração da senha)
                }
            }
        }

        return true;
    }
}
