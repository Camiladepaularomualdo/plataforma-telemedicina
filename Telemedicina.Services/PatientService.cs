using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telemedicina.Domain.Entities;
using Telemedicina.Interfaces;

namespace Telemedicina.Services;

public class PatientService : IPatientService
{
    private readonly IPatientRepository _repository;

    public PatientService(IPatientRepository repository)
    {
        _repository = repository;
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
}
