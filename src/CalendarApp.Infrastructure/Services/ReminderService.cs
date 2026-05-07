using CalendarApp.Domain.Entities;
using CalendarApp.Domain.Interfaces;
using CalendarApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CalendarApp.Infrastructure.Services;

public class ReminderService : IReminderService
{
    private readonly AppDbContext _context;

    public ReminderService(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddReminderAsync(Appointment appointment, DateTime reminderTime)
    {
        var reminder = new Reminder
        {
            AppointmentId = appointment.Id,
            Appointment = appointment,
            ReminderTime = reminderTime,
            IsSent = false
        };

        _context.Reminders.Add(reminder);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Reminder>> GetPendingRemindersAsync(int userId)
    {
        return await _context.Reminders
            .Include(r => r.Appointment)
            .Where(r => r.Appointment.UserId == userId && !r.IsSent)
            .OrderBy(r => r.ReminderTime)
            .ToListAsync();
    }

    public async Task MarkAsSentAsync(int reminderId)
    {
        var reminder = await _context.Reminders.FindAsync(reminderId);
        if (reminder != null)
        {
            reminder.IsSent = true;
            await _context.SaveChangesAsync();
        }
    }
}
