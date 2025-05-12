using APBD_T.Infrastructure.DTOs;

namespace APBD_T.Application.Interfaces;

public interface IAppointmentService
{
    Task<GetAppointmentDto?> GetAppointmentByIdAsync(int id);
    Task<(bool isSuccess, string message)> AddAppointmentAsync(AddAppointmentDto appointmentDto);
}