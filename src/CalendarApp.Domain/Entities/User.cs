namespace CalendarApp.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<GroupMeeting> GroupMeetings { get; set; } = new List<GroupMeeting>();
}
