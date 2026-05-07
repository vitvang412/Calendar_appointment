using CalendarApp.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CalendarApp.Web.Controllers;

public class CalendarController : Controller
{
    private readonly IAppointmentRepository _appointmentRepo;

    public CalendarController(IAppointmentRepository appointmentRepo)
    {
        _appointmentRepo = appointmentRepo;
    }

    public async Task<IActionResult> Index(DateTime? date)
    {
        var targetDate = date ?? DateTime.Today;
        var userId = 1; // Simplified: use first user

        var startDate = new DateTime(targetDate.Year, targetDate.Month, 1);
        var endDate = startDate.AddMonths(1);

        var appointments = await _appointmentRepo.GetByDateRangeAsync(userId, startDate, endDate);

        ViewBag.CurrentDate = targetDate;
        ViewBag.MonthStart = startDate;
        ViewBag.MonthEnd = endDate;

        return View(appointments);
    }
}
