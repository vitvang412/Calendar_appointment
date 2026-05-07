using CalendarApp.Domain.Entities;

namespace CalendarApp.Domain.Interfaces;

public interface IGroupMeetingRepository
{
    Task<GroupMeeting?> GetByIdAsync(int id);
    Task<GroupMeeting?> FindByNameAndDurationAsync(string name, TimeSpan duration);
    Task AddAsync(GroupMeeting groupMeeting);
    Task UpdateAsync(GroupMeeting groupMeeting);
}
