using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using S4N.LunchToHome.Application.Common;
using S4N.LunchToHome.Application.Deliveries.Commands.SendDelivery;
using S4N.LunchToHome.Domain.Entities;

namespace S4N.LunchToHome.Application.Tests.Deliveries.Commands
{
    [TestFixture]
    public class SendDeliveryCommandValidatorTests
    {
        private SendDeliveryCommandValidator validator;

        private SendDeliveryCommand model;

        [SetUp]
        public void SetUp()
        {
            this.validator = new SendDeliveryCommandValidator();

            this.model = new SendDeliveryCommand();
        }

        [Test]
        public void Validate_EmptyModel_ShouldFail()
        {
            var result = this.validator.TestValidate(this.model);

            result.ShouldHaveValidationErrorFor("DeliveryId");
        }


        [Test]
        public void Validate_ExistentDelivery_ShouldPass()
        {
            this.model.DeliveryId = Guid.NewGuid();

            var result = this.validator.TestValidate(this.model);

            result.ShouldNotHaveValidationErrorFor("DeliveryId");
        }
    }
}