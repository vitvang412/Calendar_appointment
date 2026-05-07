using CalendarApp.Domain.Entities;
using CalendarApp.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CalendarApp.Infrastructure.Data.Repositories;

public class AppointmentRepository : IAppointmentRepository
{
    private readonly AppDbContext _context;

    public AppointmentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Appointment?> GetByIdAsync(int id)
    {
        return await _context.Appointments
            .Include(a => a.User)
            .Include(a => a.GroupMeeting)
            .Include(a => a.Reminder)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<Appointment>> GetByUserIdAsync(int userId)
    {
        return await _context.Appointments
            .Include(a => a.GroupMeeting)
            .Where(a => a.UserId == userId)
            .OrderBy(a => a.StartTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<Appointment>> GetByDateRangeAsync(int userId, DateTime start, DateTime end)
    {
        return await _context.Appointments
            .Include(a => a.GroupMeeting)
            .Include(a => a.Reminder)
            .Where(a => a.UserId == userId && a.StartTime >= start && a.StartTime <= end)
            .OrderBy(a => a.StartTime)
            .ToListAsync();
    }

    public async Task AddAsync(Appointment appointment)
    {
        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Appointment appointment)
    {
        _context.Appointments.Update(appointment);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment != null)
        {
            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> HasConflictAsync(int userId, DateTime start, DateTime end, int? excludeId = null)
    {
        return await _context.Appointments
            .AnyAsync(a => a.UserId == userId
                && (excludeId == null || a.Id != excludeId.Value)
                && a.StartTime < end
                && a.EndTime > start);
    }

    public async Task<Appointment?> FindConflictingAsync(int userId, DateTime start, DateTime end)
    {
        return await _context.Appointments
            .FirstOrDefaultAsync(a => a.UserId == userId
                && a.StartTime < end
                && a.EndTime > start);
    }
}
