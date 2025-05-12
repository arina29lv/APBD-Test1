using APBD_T.Domain.Models;
using Microsoft.Data.SqlClient;

namespace APBD_T.Domain.Interfaces;

public interface IPatientRepository
{
    Task<Patient?> GetPatientByIdAsync(int patientId, SqlConnection connection, SqlTransaction transaction);
}