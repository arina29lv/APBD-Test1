using APBD_T.Domain.Models;
using APBD_T.Infrastructure.DTOs;
using Microsoft.Data.SqlClient;

namespace APBD_T.Domain.Interfaces;

public interface IServiceRepository
{
    Task<IEnumerable<ServiceDto>> GetAllServicesAsync(int appointmentId);
    Task<Service?> GetServiceByNameAsync(string name, SqlConnection connection, SqlTransaction transaction);
}