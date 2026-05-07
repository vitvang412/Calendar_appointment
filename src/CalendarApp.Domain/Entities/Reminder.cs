namespace CalendarApp.Domain.Entities;

public class Reminder
{
    public int Id { get; set; }
    public int AppointmentId { get; set; }
    public Appointment Appointment { get; set; } = null!;
    public DateTime ReminderTime { get; set; }
    public bool IsSent { get; set; }
}
