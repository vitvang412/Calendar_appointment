using CalendarApp.Domain.Entities;

namespace CalendarApp.Application.DTOs;

public class AddAppointmentResult
{
    public bool IsSuccess { get; private set; }
    public bool IsValidationFailed { get; private set; }
    public bool IsConflictDetected { get; private set; }
    public bool IsGroupMeetingFound { get; private set; }

    public Appointment? Appointment { get; private set; }
    public string? ErrorMessage { get; private set; }
    public Appointment? ConflictingAppointment { get; private set; }
    public GroupMeeting? GroupMeeting { get; private set; }

    public static AddAppointmentResult Success(Appointment appointment) => new()
    {
        IsSuccess = true,
        Appointment = appointment
    };

    public static AddAppointmentResult ValidationFailed(string error) => new()
    {
        IsValidationFailed = true,
        ErrorMessage = error
    };

    public static AddAppointmentResult ConflictDetected(Appointment conflicting) => new()
    {
        IsConflictDetected = true,
        ConflictingAppointment = conflicting,
        ErrorMessage = "You already have an appointment at this time."
    };

    public static AddAppointmentResult GroupMeetingFound(GroupMeeting group) => new()
    {
        IsGroupMeetingFound = true,
        GroupMeeting = group
    };
}
