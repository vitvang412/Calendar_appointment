using FluentValidation;

namespace CalendarApp.Application.UseCases.AddAppointment;

public class AddAppointmentValidator : AbstractValidator<AddAppointmentCommand>
{
    public AddAppointmentValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Appointment name is required.");

        RuleFor(x => x.EndTime)
            .GreaterThan(x => x.StartTime)
            .WithMessage("End time must be after start time.");

        RuleFor(x => x.StartTime)
            .NotEmpty().WithMessage("Start time is required.");

        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("Valid user is required.");
    }
}
