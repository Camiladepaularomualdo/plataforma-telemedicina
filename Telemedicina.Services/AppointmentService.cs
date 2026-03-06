using System.Collections.Generic;
using System.Threading.Tasks;
using Telemedicina.Domain.Entities;
using Telemedicina.Interfaces;

namespace Telemedicina.Services;

public class AppointmentService : IAppointmentService
{
    private readonly IAppointmentRepository _repository;

    public AppointmentService(IAppointmentRepository repository)
    {
        _repository = repository;
    }

    public async Task<Appointment> CreateAppointmentAsync(Appointment appointment)
    {
        // Simple logic for booking
        appointment.Status = Domain.Enums.AppointmentStatus.Agendado;
        await _repository.AddAsync(appointment);
        await _repository.SaveChangesAsync();

        return appointment;
    }

    public async Task<IEnumerable<Appointment>> GetDoctorAppointmentsByMonthAsync(int doctorId, int year, int month)
    {
        return await _repository.GetDoctorAppointmentsByMonthAsync(doctorId, year, month);
    }

    public async Task<IEnumerable<Appointment>> GetDoctorAppointmentsAsync(int doctorId)
    {
        return await _repository.GetDoctorAppointmentsAsync(doctorId);
    }

    public async Task<bool> UpdateStatusAsync(int appointmentId, Telemedicina.Domain.Enums.AppointmentStatus status)
    {
        var appointment = await _repository.GetByIdAsync(appointmentId);
        if (appointment == null) return false;

        appointment.Status = status;
        _repository.Update(appointment);
        await _repository.SaveChangesAsync();
        return true;
    }
}
