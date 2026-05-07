using CalendarApp.Domain.Entities;
using CalendarApp.Domain.Interfaces;

namespace CalendarApp.Application.UseCases.JoinGroupMeeting;

public class JoinGroupMeetingHandler
{
    private readonly IConflictResolver _conflictResolver;
    private readonly IAppointmentRepository _appointmentRepo;
    private readonly IGroupMeetingRepository _groupMeetingRepo;

    public JoinGroupMeetingHandler(
        IConflictResolver conflictResolver,
        IAppointmentRepository appointmentRepo,
        IGroupMeetingRepository groupMeetingRepo)
    {
        _conflictResolver = conflictResolver;
        _appointmentRepo = appointmentRepo;
        _groupMeetingRepo = groupMeetingRepo;
    }

    public async Task<Appointment> HandleAsync(JoinGroupMeetingCommand cmd)
    {
        var user = new User { Id = cmd.UserId };

        // Bước 12: Add participant to group meeting
        var groupMeeting = await _groupMeetingRepo.GetByIdAsync(cmd.GroupMeetingId);
        if (groupMeeting != null)
        {
            groupMeeting.AddParticipant(user);
            await _groupMeetingRepo.UpdateAsync(groupMeeting);
        }

        // Bước 13: Add appointment to personal calendar
        var appointment = new Appointment
        {
            Name = cmd.AppointmentName,
            Location = cmd.AppointmentLocation,
            StartTime = cmd.AppointmentStartTime,
            EndTime = cmd.AppointmentEndTime,
            UserId = cmd.UserId,
            GroupMeetingId = cmd.GroupMeetingId
        };

        await _appointmentRepo.AddAsync(appointment);
        return appointment;
    }
}
