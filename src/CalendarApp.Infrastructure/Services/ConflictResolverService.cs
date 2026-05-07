using CalendarApp.Domain.Entities;
using CalendarApp.Domain.Interfaces;
using CalendarApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CalendarApp.Infrastructure.Services;

public class ConflictResolverService : IConflictResolver
{
    private readonly AppDbContext _context;
    private readonly IAppointmentRepository _appointmentRepo;
    private readonly IGroupMeetingRepository _groupMeetingRepo;

    public ConflictResolverService(
        AppDbContext context,
        IAppointmentRepository appointmentRepo,
        IGroupMeetingRepository groupMeetingRepo)
    {
        _context = context;
        _appointmentRepo = appointmentRepo;
        _groupMeetingRepo = groupMeetingRepo;
    }

    public async Task<bool> CheckConflictAsync(int userId, DateTime start, DateTime end, int? excludeAppointmentId = null)
    {
        return await _appointmentRepo.HasConflictAsync(userId, start, end, excludeAppointmentId);
    }

    public async Task<GroupMeeting?> FindMatchingGroupMeetingAsync(string name, TimeSpan duration)
    {
        return await _groupMeetingRepo.FindByNameAndDurationAsync(name, duration);
    }

    public async Task ReplaceAppointmentAsync(int oldAppointmentId, Appointment newAppointment)
    {
        await _appointmentRepo.DeleteAsync(oldAppointmentId);
        await _appointmentRepo.AddAsync(newAppointment);
    }


}
