using APBD_T.Domain.Models;
using Microsoft.Data.SqlClient;

namespace APBD_T.Domain.Interfaces;

public interface IDoctorRepository
{
    Task<Doctor?> GetDoctorByPwzAsync(string pwz, SqlConnection connection, SqlTransaction transaction);
}