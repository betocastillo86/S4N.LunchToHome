using FluentValidation;
using S4N.LunchToHome.Domain.ValueObjects;

namespace S4N.LunchToHome.Application.Deliveries.Models
{
    public class RouteModelValidator : AbstractValidator<RouteModel>
    {
        public RouteModelValidator()
        {
            this.RuleFor(c => c.Path)
                .Must(this.IsValidPath)
                .WithMessage("Invalid path structure");
        }

        private bool IsValidPath(string path)
        {
            return Route.IsValidPath(path);
        }
    }
}