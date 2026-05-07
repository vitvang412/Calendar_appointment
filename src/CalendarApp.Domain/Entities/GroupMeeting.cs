namespace CalendarApp.Domain.Entities;

public class GroupMeeting
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public TimeSpan Duration { get; set; }
    public DateTime ScheduledTime { get; set; }

    public ICollection<User> Participants { get; set; } = new List<User>();
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public void AddParticipant(User user)
    {
        if (!Participants.Any(p => p.Id == user.Id))
            Participants.Add(user);
    }
}
