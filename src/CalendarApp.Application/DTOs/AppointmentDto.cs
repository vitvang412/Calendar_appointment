namespace CalendarApp.Application.DTOs;

public record AppointmentDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Location { get; init; } = string.Empty;
    public DateTime StartTime { get; init; }
    public DateTime EndTime { get; init; }
    public string UserName { get; init; } = string.Empty;
    public string? GroupMeetingName { get; init; }
    public bool HasReminder { get; init; }
}
