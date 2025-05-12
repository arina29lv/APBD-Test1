using System.Data;
using APBD_T.Domain.Interfaces;
using APBD_T.Domain.Models;
using Microsoft.Data.SqlClient;

namespace APBD_T.Infrastructure.Repositories;

public class PatientRepository : IPatientRepository
{
    private readonly SqlConnection _connection;

    public PatientRepository(SqlConnection connection)
    {
        _connection = connection;
    }

    public async Task<Patient?> GetPatientByIdAsync(int patientId, SqlConnection connection, SqlTransaction transaction)
    {
        const string query = 
            @"Select * from Patient where patient_id = @patientId";
        
        await using var command = new SqlCommand(query, _connection, transaction);
        command.Parameters.AddWithValue("@patientId", patientId);
        
        if (_connection.State != ConnectionState.Open)
            await _connection.OpenAsync();
        
        await using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new Patient
            {
                PatientId = reader.GetInt32(0),
                FirstName = reader.GetString(1),
                LastName = reader.GetString(2),
                DateOfBirth = reader.GetDateTime(3),
            };
        }
        
        return null;
    }
}