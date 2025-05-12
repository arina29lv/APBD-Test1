namespace APBD_T.Infrastructure.DTOs;

public class AddAppointmentDto
{
    public int AppointmentId { get; set; }
    public int PatientId { get; set; }
    public string Pwz {get; set;}
    public List<AddServiceDto> Services { get; set; }
}