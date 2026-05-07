namespace CalendarApp.Domain.Entities;

public class Appointment
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int? GroupMeetingId { get; set; }
    public GroupMeeting? GroupMeeting { get; set; }

    public Reminder? Reminder { get; set; }

    public TimeSpan Duration => EndTime - StartTime;

    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(Name) && Duration > TimeSpan.Zero;
    }
}
