using CalendarApp.Application.DTOs;
using CalendarApp.Domain.Entities;
using CalendarApp.Domain.Interfaces;

namespace CalendarApp.Application.UseCases.AddAppointment;

public class AddAppointmentHandler
{
    private readonly IAppointmentRepository _appointmentRepo;
    private readonly IConflictResolver _conflictResolver;
    private readonly IReminderService _reminderService;
    private readonly AddAppointmentValidator _validator = new();

    public AddAppointmentHandler(
        IAppointmentRepository appointmentRepo,
        IConflictResolver conflictResolver,
        IReminderService reminderService)
    {
        _appointmentRepo = appointmentRepo;
        _conflictResolver = conflictResolver;
        _reminderService = reminderService;
    }

    public async Task<AddAppointmentResult> HandleAsync(AddAppointmentCommand cmd)
    {
        // Bước 4: Validation
        var validationResult = await _validator.ValidateAsync(cmd);
        if (!validationResult.IsValid)
        {
            var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            return AddAppointmentResult.ValidationFailed(errors);
        }

        var appointment = new Appointment
        {
            Name = cmd.Name,
            Location = cmd.Location,
            StartTime = cmd.StartTime,
            EndTime = cmd.EndTime,
            UserId = cmd.UserId
        };

        if (!appointment.IsValid())
            return AddAppointmentResult.ValidationFailed("Invalid appointment data.");

        // Bước 6: Check conflict
        var hasConflict = await _conflictResolver.CheckConflictAsync(
            cmd.UserId, cmd.StartTime, cmd.EndTime);

        if (hasConflict)
        {
            var conflicting = await _appointmentRepo.FindConflictingAsync(
                cmd.UserId, cmd.StartTime, cmd.EndTime);
            return AddAppointmentResult.ConflictDetected(conflicting!);
        }

        // Bước 9: Check group meeting
        var groupMatch = await _conflictResolver.FindMatchingGroupMeetingAsync(
            cmd.Name, appointment.Duration);

        if (groupMatch != null)
            return AddAppointmentResult.GroupMeetingFound(groupMatch);

        // Bước 16: Save appointment first (so Id is generated)
        await _appointmentRepo.AddAsync(appointment);

        // Bước 14-15: Add reminder (after appointment is saved, so AppointmentId is valid)
        if (cmd.AddReminder)
            await _reminderService.AddReminderAsync(appointment, cmd.StartTime.AddMinutes(-15));

        return AddAppointmentResult.Success(appointment);
    }
}
