using System.Data;
using APBD_T.Application.Interfaces;
using APBD_T.Domain.Interfaces;
using APBD_T.Infrastructure.DTOs;
using Microsoft.Data.SqlClient;

namespace APBD_T.Application.Services;

public class AppointmentService : IAppointmentService
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IServiceRepository _serviceRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly IDoctorRepository _doctorRepository;
    private readonly SqlConnection _sqlConnection;

    public AppointmentService(
        IAppointmentRepository appointmentRepository,
        IServiceRepository serviceRepository,
        IPatientRepository patientRepository,
        IDoctorRepository doctorRepository,
        SqlConnection sqlConnection)
    {
        _appointmentRepository = appointmentRepository;
        _serviceRepository = serviceRepository;
        _patientRepository = patientRepository;
        _doctorRepository = doctorRepository;
        _sqlConnection = sqlConnection;
    }
    
    public async Task<GetAppointmentDto?> GetAppointmentByIdAsync(int id)
    {
        var appointments = await _appointmentRepository.GetAppointmentByIdAsync(id);
        if (appointments == null)
            return null;

        appointments.Services = await _serviceRepository.GetAllServicesAsync(id);
        return appointments;
    }

    public async Task<(bool isSuccess, string message)> AddAppointmentAsync(AddAppointmentDto appointmentDto)
    {
        if (_sqlConnection.State != ConnectionState.Open)
            await _sqlConnection.OpenAsync();

        await using var transaction = (SqlTransaction)await _sqlConnection.BeginTransactionAsync();

        try
        {
            var appointment = await _appointmentRepository.GetAppointmentByIdForCheck(appointmentDto.AppointmentId, _sqlConnection, transaction);
            if (appointment is not null)
                return (false, "Appointment already exists");
        
            var patient = await _patientRepository.GetPatientByIdAsync(appointmentDto.PatientId, _sqlConnection, transaction);
            if (patient is null)
                return (false, "Patient not found");

            var doctor = await _doctorRepository.GetDoctorByPwzAsync(appointmentDto.Pwz, _sqlConnection, transaction);
            if (doctor is null)
                return (false, "Doctor not found");

            var serviceList = new List<(int serviceId, decimal serviceFee)>();
            foreach (var service in appointmentDto.Services)
            {
                var serviceItem = await _serviceRepository.GetServiceByNameAsync(service.ServiceName, _sqlConnection, transaction);
                if (serviceItem is null)
                    return (false, "Service not found");
            
                serviceList.Add((serviceItem.ServiceId, service.ServiceFee));
            }
        
            await _appointmentRepository.CreateAppointmentAsync(appointmentDto.AppointmentId, appointmentDto.PatientId, doctor.DoctorId, _sqlConnection, transaction);
            foreach (var service in serviceList)
            {
                await _appointmentRepository.CreateAppointmentServiceAsync(appointmentDto.AppointmentId, service.serviceId, service.serviceFee, _sqlConnection, transaction);
            }
            
            await transaction.CommitAsync();
            return (true, "Appointment added");
            
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return (false, $"Transaction failed: {ex.Message}");
        }
    }
}