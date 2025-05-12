namespace APBD_T.Infrastructure.DTOs;

public class GetAppointmentDto
{
    public DateTime Date { get; set; }
    public PatientDto Patient{ get; set; }
    public DoctorDto Doctor { get; set; }
    public IEnumerable<ServiceDto> Services { get; set; } = new List<ServiceDto>();
}