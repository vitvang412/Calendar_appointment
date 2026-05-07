namespace CalendarApp.Application.UseCases.JoinGroupMeeting;

public record JoinGroupMeetingCommand
{
    public int GroupMeetingId { get; init; }
    public int UserId { get; init; }
    public DateTime AppointmentStartTime { get; init; }
    public DateTime AppointmentEndTime { get; init; }
    public string AppointmentName { get; init; } = string.Empty;
    public string AppointmentLocation { get; init; } = string.Empty;
}
