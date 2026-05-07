using CalendarApp.Domain.Entities;

namespace CalendarApp.Domain.Interfaces;

public interface IConflictResolver
{
    Task<bool> CheckConflictAsync(int userId, DateTime start, DateTime end, int? excludeAppointmentId = null);
    Task<GroupMeeting?> FindMatchingGroupMeetingAsync(string name, TimeSpan duration);
    Task ReplaceAppointmentAsync(int oldAppointmentId, Appointment newAppointment);

}
