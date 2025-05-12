using System.Data;
using APBD_T.Domain.Interfaces;
using APBD_T.Domain.Models;
using Microsoft.Data.SqlClient;

namespace APBD_T.Infrastructure.Repositories;

public class DoctorRepository : IDoctorRepository
{
    private readonly SqlConnection _sqlConnection;

    public DoctorRepository(SqlConnection sqlConnection)
    {
        _sqlConnection = sqlConnection;
    }
    
    public async Task<Doctor?> GetDoctorByPwzAsync(string pwz, SqlConnection connection, SqlTransaction transaction)
    {
        const string query = 
            @"Select * from Doctor where PWZ = @Pwz";
        
        await using var command = new SqlCommand(query, _sqlConnection, transaction);
        command.Parameters.AddWithValue("@Pwz", pwz);
        
        if (_sqlConnection.State != ConnectionState.Open)
            await _sqlConnection.OpenAsync();
        
        await using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new Doctor
            {
                DoctorId = reader.GetInt32(0),
                FirstName = reader.GetString(1),
                LastName = reader.GetString(2),
                PWZ = reader.GetString(3),
            };
        }  
        
        return null;
    }
}