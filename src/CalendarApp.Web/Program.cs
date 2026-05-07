using CalendarApp.Application.UseCases.AddAppointment;
using CalendarApp.Application.UseCases.JoinGroupMeeting;
using CalendarApp.Domain.Interfaces;
using CalendarApp.Infrastructure.Data;
using CalendarApp.Infrastructure.Data.Repositories;
using CalendarApp.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// MySQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 21))));

// DI - Repositories
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<IGroupMeetingRepository, GroupMeetingRepository>();

// DI - Services
builder.Services.AddScoped<IConflictResolver, ConflictResolverService>();
builder.Services.AddScoped<IReminderService, ReminderService>();

// DI - Use Cases
builder.Services.AddScoped<AddAppointmentHandler>();
builder.Services.AddScoped<JoinGroupMeetingHandler>();

var app = builder.Build();

// Auto-migrate and seed data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();

    if (!db.Users.Any())
    {
        var user = new CalendarApp.Domain.Entities.User { Name = "Demo User", Email = "demo@example.com" };
        db.Users.Add(user);
        db.SaveChanges();

        // Seed some sample data
        var today = DateTime.Today;
        db.Appointments.AddRange(
            new CalendarApp.Domain.Entities.Appointment
            {
                Name = "Team Standup",
                Location = "Conference Room A",
                StartTime = today.AddHours(9),
                EndTime = today.AddHours(9).AddMinutes(30),
                UserId = user.Id
            },
            new CalendarApp.Domain.Entities.Appointment
            {
                Name = "Project Review",
                Location = "Online",
                StartTime = today.AddHours(14),
                EndTime = today.AddHours(15),
                UserId = user.Id
            }
        );

        // Seed a group meeting
        var group = new CalendarApp.Domain.Entities.GroupMeeting
        {
            Name = "Sprint Planning",
            Duration = TimeSpan.FromHours(1),
            ScheduledTime = today.AddDays(1).AddHours(10)
        };
        db.GroupMeetings.Add(group);
        db.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Calendar}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
