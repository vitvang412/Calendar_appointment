using CalendarApp.Domain.Entities;
using CalendarApp.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CalendarApp.Infrastructure.Data.Repositories;

public class GroupMeetingRepository : IGroupMeetingRepository
{
    private readonly AppDbContext _context;

    public GroupMeetingRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<GroupMeeting?> GetByIdAsync(int id)
    {
        return await _context.GroupMeetings
            .Include(g => g.Participants)
            .FirstOrDefaultAsync(g => g.Id == id);
    }

    public async Task<GroupMeeting?> FindByNameAndDurationAsync(string name, TimeSpan duration)
    {
        return await _context.GroupMeetings
            .Include(g => g.Participants)
            .FirstOrDefaultAsync(g => g.Name == name && g.Duration == duration);
    }

    public async Task AddAsync(GroupMeeting groupMeeting)
    {
        _context.GroupMeetings.Add(groupMeeting);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(GroupMeeting groupMeeting)
    {
        _context.GroupMeetings.Update(groupMeeting);
        await _context.SaveChangesAsync();
    }
}
