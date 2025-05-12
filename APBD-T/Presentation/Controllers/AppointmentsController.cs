using APBD_T.Application.Interfaces;
using APBD_T.Infrastructure.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace APBD_T.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    
    private readonly IAppointmentService _appointmentService;

    public AppointmentsController(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAppointmentsByIdAsync(int id)
    {
        var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
        return appointment == null ? NotFound() : Ok(appointment);
        
    }

    [HttpPost]
    public async Task<IActionResult> AddAppointment([FromBody] AddAppointmentDto dto)
    {
        var res = await _appointmentService.AddAppointmentAsync(dto);
        return res.isSuccess ? Ok(new { message = res.message}) : BadRequest(new { error = res.message});
    }
}