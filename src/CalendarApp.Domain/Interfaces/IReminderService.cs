using CalendarApp.Domain.Entities;

namespace CalendarApp.Domain.Interfaces;

public interface IReminderService
{
    Task AddReminderAsync(Appointment appointment, DateTime reminderTime);
    Task<IEnumerable<Reminder>> GetPendingRemindersAsync(int userId);
    Task MarkAsSentAsync(int reminderId);
}
