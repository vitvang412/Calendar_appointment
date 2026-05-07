using CalendarApp.Domain.Entities;

namespace CalendarApp.Domain.Interfaces;

public interface IAppointmentRepository
{
    Task<Appointment?> GetByIdAsync(int id);
    Task<IEnumerable<Appointment>> GetByUserIdAsync(int userId);
    Task<IEnumerable<Appointment>> GetByDateRangeAsync(int userId, DateTime start, DateTime end);
    Task AddAsync(Appointment appointment);
    Task UpdateAsync(Appointment appointment);
    Task DeleteAsync(int id);
    Task<bool> HasConflictAsync(int userId, DateTime start, DateTime end, int? excludeId = null);
    Task<Appointment?> FindConflictingAsync(int userId, DateTime start, DateTime end);
}
