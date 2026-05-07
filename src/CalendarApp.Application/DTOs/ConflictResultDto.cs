using CalendarApp.Domain.Entities;

namespace CalendarApp.Application.DTOs;

public record ConflictResultDto
{
    public bool HasConflict { get; init; }
    public Appointment? ConflictingAppointment { get; init; }
    public string? Message { get; init; }
}
