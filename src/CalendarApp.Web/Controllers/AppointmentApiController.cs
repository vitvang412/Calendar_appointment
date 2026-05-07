using CalendarApp.Application.UseCases.AddAppointment;
using CalendarApp.Application.UseCases.JoinGroupMeeting;
using CalendarApp.Domain.Enums;
using CalendarApp.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CalendarApp.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentApiController : ControllerBase
{
    private readonly AddAppointmentHandler _addHandler;
    private readonly JoinGroupMeetingHandler _joinHandler;
    private readonly IConflictResolver _conflictResolver;
    private readonly IAppointmentRepository _appointmentRepo;
    private readonly IReminderService _reminderService;

    public AppointmentApiController(
        AddAppointmentHandler addHandler,
        JoinGroupMeetingHandler joinHandler,
        IConflictResolver conflictResolver,
        IAppointmentRepository appointmentRepo,
        IReminderService reminderService)
    {
        _addHandler = addHandler;
        _joinHandler = joinHandler;
        _conflictResolver = conflictResolver;
        _appointmentRepo = appointmentRepo;
        _reminderService = reminderService;
    }

    // Fix timezone: strip UTC Kind so DB stores local time as-is
    private static DateTime AsLocal(DateTime dt) =>
        dt.Kind == DateTimeKind.Utc
            ? DateTime.SpecifyKind(dt, DateTimeKind.Unspecified)
            : DateTime.SpecifyKind(dt, DateTimeKind.Unspecified);

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] AddAppointmentCommand cmd)
    {
        cmd = cmd with { StartTime = AsLocal(cmd.StartTime), EndTime = AsLocal(cmd.EndTime) };
        var result = await _addHandler.HandleAsync(cmd);

        if (result.IsValidationFailed)
            return BadRequest(new { error = result.ErrorMessage });

        if (result.IsConflictDetected)
            return Conflict(new
            {
                message = result.ErrorMessage,
                conflictingAppointment = new
                {
                    result.ConflictingAppointment!.Id,
                    result.ConflictingAppointment.Name,
                    result.ConflictingAppointment.StartTime,
                    result.ConflictingAppointment.EndTime
                }
            });

        if (result.IsGroupMeetingFound)
            return Ok(new
            {
                action = "join_group",
                groupMeeting = new
                {
                    result.GroupMeeting!.Id,
                    result.GroupMeeting.Name,
                    result.GroupMeeting.Duration,
                    ParticipantCount = result.GroupMeeting.Participants.Count
                }
            });

        return Ok(new
        {
            message = "Appointment saved successfully!",
            appointment = new
            {
                result.Appointment!.Id,
                result.Appointment.Name,
                result.Appointment.StartTime,
                result.Appointment.EndTime
            }
        });
    }

    [HttpPost("resolve-conflict")]
    public async Task<IActionResult> ResolveConflict([FromBody] ConflictResolutionRequest request)
    {
        if (request.Option == ConflictOption.ReplaceExisting)
        {
            var startTime = AsLocal(request.Command.StartTime);
            var endTime   = AsLocal(request.Command.EndTime);
            var duration  = endTime - startTime;

            // ✅ Check group meeting FIRST (same logic as AddAppointment)
            var groupMeeting = await _conflictResolver.FindMatchingGroupMeetingAsync(request.Command.Name, duration);
            if (groupMeeting != null)
            {
                // Delete the conflicting appointment ONLY after we know we'll handle it
                await _appointmentRepo.DeleteAsync(request.ConflictingAppointmentId);

                return Ok(new
                {
                    action = "join_group",
                    groupMeeting = new
                    {
                        groupMeeting.Id,
                        groupMeeting.Name,
                        groupMeeting.Duration,
                        ParticipantCount = groupMeeting.Participants.Count
                    }
                });
            }

            // No group meeting match → replace normally
            var newAppointment = new CalendarApp.Domain.Entities.Appointment
            {
                Name      = request.Command.Name,
                Location  = request.Command.Location,
                StartTime = startTime,
                EndTime   = endTime,
                UserId    = request.Command.UserId
            };

            await _conflictResolver.ReplaceAppointmentAsync(request.ConflictingAppointmentId, newAppointment);

            if (request.Command.AddReminder)
            {
                var reminderService = HttpContext.RequestServices
                    .GetRequiredService<CalendarApp.Domain.Interfaces.IReminderService>();
                await reminderService.AddReminderAsync(newAppointment, newAppointment.StartTime.AddMinutes(-15));
            }

            return Ok(new { message = "Appointment replaced successfully!" });
        }

        return BadRequest(new { message = "Invalid option." });
    }

    [HttpPost("join-group")]
    public async Task<IActionResult> JoinGroup([FromBody] JoinGroupMeetingCommand cmd)
    {
        cmd = cmd with
        {
            AppointmentStartTime = AsLocal(cmd.AppointmentStartTime),
            AppointmentEndTime = AsLocal(cmd.AppointmentEndTime)
        };
        var appointment = await _joinHandler.HandleAsync(cmd);
        return Ok(new
        {
            message = "Joined group meeting successfully!",
            appointment = new
            {
                appointment.Id,
                appointment.Name,
                appointment.StartTime,
                appointment.EndTime
            }
        });
    }

    // Sequence Diagram: [no group meeting match] → Calendar.addAppointment(appt)
    // Called when user skips group meeting — saves directly without triggering group check again
    [HttpPost("save-personal")]
    public async Task<IActionResult> SavePersonal([FromBody] AddAppointmentCommand cmd)
    {
        cmd = cmd with { StartTime = AsLocal(cmd.StartTime), EndTime = AsLocal(cmd.EndTime) };

        if (string.IsNullOrWhiteSpace(cmd.Name))
            return BadRequest(new { error = "Name is required." });

        var appointment = new CalendarApp.Domain.Entities.Appointment
        {
            Name = cmd.Name,
            Location = cmd.Location,
            StartTime = cmd.StartTime,
            EndTime = cmd.EndTime,
            UserId = cmd.UserId
        };

        await _appointmentRepo.AddAsync(appointment);

        if (cmd.AddReminder)
        {
            var reminderService = HttpContext.RequestServices
                .GetRequiredService<CalendarApp.Domain.Interfaces.IReminderService>();
            await reminderService.AddReminderAsync(appointment, appointment.StartTime.AddMinutes(-15));
        }

        return Ok(new
        {
            message = "Personal appointment saved successfully!",
            appointment = new { appointment.Id, appointment.Name, appointment.StartTime, appointment.EndTime }
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var appointment = await _appointmentRepo.GetByIdAsync(id);
        if (appointment == null)
            return NotFound();

        await _appointmentRepo.DeleteAsync(id);
        return Ok(new { message = "Appointment deleted." });
    }

    // Trả về các reminder đến hạn (ReminderTime <= now) và đánh dấu đã gửi
    [HttpGet("reminders")]
    public async Task<IActionResult> GetReminders()
    {
        var reminders = await _reminderService.GetPendingRemindersAsync(1);
        var now = DateTime.Now;
        var due = reminders.Where(r => r.ReminderTime <= now).ToList();
        foreach (var r in due)
            await _reminderService.MarkAsSentAsync(r.Id);

        return Ok(due.Select(r => new
        {
            r.Id,
            name = r.Appointment.Name,
            r.ReminderTime
        }));
    }
}

public record ConflictResolutionRequest
{
    public AddAppointmentCommand Command { get; init; } = null!;
    public int ConflictingAppointmentId { get; init; }
    public ConflictOption Option { get; init; }
}
