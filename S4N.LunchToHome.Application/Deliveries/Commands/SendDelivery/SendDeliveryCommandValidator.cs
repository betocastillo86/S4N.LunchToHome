using System;
using FluentValidation;

namespace S4N.LunchToHome.Application.Deliveries.Commands.SendDelivery
{
    public class SendDeliveryCommandValidator : AbstractValidator<SendDeliveryCommand>
    {
        public SendDeliveryCommandValidator()
        {
            this.RuleFor(c => c.DeliveryId)
                .NotEqual(Guid.Empty);
        }
    }
}