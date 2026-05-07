namespace CalendarApp.Application.UseCases.CheckConflict;

public record CheckConflictQuery
{
    public int UserId { get; init; }
    public DateTime StartTime { get; init; }
    public DateTime EndTime { get; init; }
    public int? ExcludeAppointmentId { get; init; }
}
