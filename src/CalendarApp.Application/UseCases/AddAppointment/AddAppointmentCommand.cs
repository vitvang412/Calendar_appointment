namespace CalendarApp.Application.UseCases.AddAppointment;

public record AddAppointmentCommand
{
    public string Name { get; init; } = string.Empty;
    public string Location { get; init; } = string.Empty;
    public DateTime StartTime { get; init; }
    public DateTime EndTime { get; init; }
    public bool AddReminder { get; init; }
    public int UserId { get; init; }
}
