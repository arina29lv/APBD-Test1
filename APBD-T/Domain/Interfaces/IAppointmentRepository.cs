using APBD_T.Domain.Models;
using APBD_T.Infrastructure.DTOs;
using Microsoft.Data.SqlClient;

namespace APBD_T.Domain.Interfaces;

public interface IAppointmentRepository
{
    Task<GetAppointmentDto?> GetAppointmentByIdAsync (int id);
    
    Task<Appointment?> GetAppointmentByIdForCheck(int id, SqlConnection connection, SqlTransaction transaction);
    Task CreateAppointmentAsync(int appointmentId, int patientId, int doctorId, SqlConnection connection, SqlTransaction transaction);
    Task CreateAppointmentServiceAsync(int appointmentId, int serviceId, decimal serviceFee, SqlConnection connection, SqlTransaction transaction);
}