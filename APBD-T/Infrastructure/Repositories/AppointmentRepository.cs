using System.Data;
using APBD_T.Domain.Interfaces;
using APBD_T.Domain.Models;
using APBD_T.Infrastructure.DTOs;
using Microsoft.Data.SqlClient;

namespace APBD_T.Infrastructure.Repositories;

public class AppointmentRepository : IAppointmentRepository
{
    private readonly SqlConnection _sqlConnection;

    public AppointmentRepository(SqlConnection sqlConnection)
    {
        _sqlConnection = sqlConnection;
    }
    
    public async Task<GetAppointmentDto?> GetAppointmentByIdAsync(int id)
    {
        const string query = @"
            SELECT 
                a.date,
                p.first_name,
                p.last_name,
                p.date_of_birth,
                d.doctor_id,
                d.pwz
            FROM Appointment a
            JOIN Patient p ON a.patient_id = p.patient_id
            JOIN Doctor d ON a.doctor_id = d.doctor_id
            WHERE a.appointment_id = @IdAppointment";

        await using var command = new SqlCommand(query, _sqlConnection);
        command.Parameters.AddWithValue("@IdAppointment", id);

        if (_sqlConnection.State != ConnectionState.Open)
            await _sqlConnection.OpenAsync();

        await using var reader = await command.ExecuteReaderAsync();

        GetAppointmentDto? result = null;

        if (await reader.ReadAsync())
        {
            result = new GetAppointmentDto
            {
                Date = reader.GetDateTime(0),
                Patient = new PatientDto
                {
                    FirstName = reader.GetString(1),
                    LastName = reader.GetString(2),
                    DateOfBirth = reader.GetDateTime(3)
                },
                Doctor = new DoctorDto
                {
                    DoctorId = reader.GetInt32(4),
                    Pwz = reader.GetString(5)
                }
            };
        }

        return result;
    }

    public async Task<Appointment?> GetAppointmentByIdForCheck(int id, SqlConnection connection, SqlTransaction transaction)
    {
        const string query = 
            @"Select * from Appointment a where a.appointment_id = @IdAppointment";
        
        await using var command = new SqlCommand(query, _sqlConnection, transaction);
        command.Parameters.AddWithValue("@IdAppointment", id);
        
        if (_sqlConnection.State != ConnectionState.Open)
            await _sqlConnection.OpenAsync();
        
        await using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new Appointment
            {
                AppointmentId = reader.GetInt32(0),
                PatientId = reader.GetInt32(1),
                DoctorId = reader.GetInt32(2),
                Date = reader.GetDateTime(3),
            };
        }
        
        return null;
    }

    public async Task CreateAppointmentAsync(int appointmentId, int patientId, int doctorId, SqlConnection connection, SqlTransaction transaction)
    {
        const string query = 
            @"Insert into Appointment (appointment_id, patient_id, doctor_id, date)
            Values (@appointmentId, @patientId, @doctorId, GETDATE())";
        
        await using var command = new SqlCommand(query, _sqlConnection, transaction);
        command.Parameters.AddWithValue("@appointmentId", appointmentId);
        command.Parameters.AddWithValue("@patientId", patientId);
        command.Parameters.AddWithValue("@doctorId", doctorId);
        
        if (_sqlConnection.State != ConnectionState.Open)
            await _sqlConnection.OpenAsync();
        
        await command.ExecuteNonQueryAsync();
    }

    public async Task CreateAppointmentServiceAsync(int appointmentId, int serviceId, decimal serviceFee, SqlConnection connection, SqlTransaction transaction)
    {
        const string query = 
            @"Insert into Appointment_Service (appointment_id, service_id, service_fee)
            Values (@appointmentId, @serviceId, @serviceFee)";
        
        await using var command = new SqlCommand(query, _sqlConnection, transaction);
        command.Parameters.AddWithValue("@appointmentId", appointmentId);
        command.Parameters.AddWithValue("@serviceId", serviceId);
        command.Parameters.AddWithValue("@serviceFee", serviceFee);
        
        if (_sqlConnection.State != ConnectionState.Open)
            await _sqlConnection.OpenAsync();
        
        await command.ExecuteNonQueryAsync();
    }
}