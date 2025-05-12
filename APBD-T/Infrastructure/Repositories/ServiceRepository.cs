using System.Data;
using APBD_T.Domain.Interfaces;
using APBD_T.Domain.Models;
using APBD_T.Infrastructure.DTOs;
using Microsoft.Data.SqlClient;

namespace APBD_T.Infrastructure.Repositories;

public class ServiceRepository : IServiceRepository
{
    private readonly SqlConnection _sqlConnection;

    public ServiceRepository(SqlConnection sqlConnection)
    {
        _sqlConnection = sqlConnection;
    }
    
    public async Task<IEnumerable<ServiceDto>> GetAllServicesAsync(int appointmentId)
    {
        const string query = "SELECT s.name, ac.service_fee FROM Service s join Appointment_Service ac on ac.service_id = s.service_id where ac.appointment_id = @apointment_id";

        await using var command = new SqlCommand(query, _sqlConnection);
        command.Parameters.AddWithValue("@apointment_id", appointmentId);
        
        if (_sqlConnection.State != ConnectionState.Open)
            await _sqlConnection.OpenAsync();

        await using var reader = await command.ExecuteReaderAsync();

        var services = new List<ServiceDto>();

        while (await reader.ReadAsync())
        {
            services.Add(new ServiceDto
            {
                Name = reader.GetString(0),
                ServiceFee = reader.GetDecimal(1)
            });
        }
        return services;
    }

    public async Task<Service?> GetServiceByNameAsync(string name, SqlConnection connection, SqlTransaction transaction)
    {
        const string query = 
            @"Select * from Service where name = @name";
        
        await using var command = new SqlCommand(query, _sqlConnection, transaction);
        command.Parameters.AddWithValue("@name", name);
        
        if (_sqlConnection.State != ConnectionState.Open)
            await _sqlConnection.OpenAsync();
        
        await using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new Service
            {
                ServiceId = reader.GetInt32(0),
                Name = reader.GetString(1),
                BaseFee = reader.GetDecimal(2),
            };
        }
        
        return null;
    }
}